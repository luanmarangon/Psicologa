using Psicologa.Application.ProntuarioSessao.ViewsModel;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Psicologa.Application.ProntuarioSessao.Services
{
    public class ApplicationProntuarioSessaoService : IDisposable
    {
        private readonly Domain.ProntuarioSessao.Services.ProntuarioSessaoService _prontuarioSessao;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly Application.Agendamento.Services.ApplicationAgentamentoService _agendamentoService;
        private readonly IAppSettings _appSettings;

        public ApplicationProntuarioSessaoService(Domain.ProntuarioSessao.Services.ProntuarioSessaoService prontuarioSessao,
            Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService,
            Application.Agendamento.Services.ApplicationAgentamentoService agendamentoService,
            IAppSettings appSettings)
        {
            _prontuarioSessao = prontuarioSessao;
            _logAplicacaoService = logAplicacaoService;
            _agendamentoService = agendamentoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) EvoluirSessao(ProntuarioSessaoViewModel sessaoVM, string[] requisicao)
        {
            var dadosExistente = _prontuarioSessao.ObterSessao(sessaoVM.Id);

            bool operacao = false;
            Domain.ProntuarioSessao.Entities.ProntuarioSessao sessao = new Domain.ProntuarioSessao.Entities.ProntuarioSessao();

            var agendamento = _agendamentoService.ObterAgendamentoPorPaciente(sessaoVM.ProntuarioId, sessaoVM.PsicologaId, sessaoVM.DataSessao);

            sessao.Id = sessaoVM.Id;
            sessao.Prontuario = new Domain.Prontuario.Entities.Prontuario
            {
                Id = sessaoVM.ProntuarioId,
            };

            sessao.Agendamento = new Domain.Agendamento.Entities.Agendamento
            {
                Id = agendamento?.Id ?? 0
            };

            sessao.DataSessao = sessaoVM.DataSessao;
            sessao.HoraInicio = TimeSpan.Parse(sessaoVM.HoraInicio);
            sessao.HoraFim = TimeSpan.Parse(sessaoVM.HoraFim);
            sessao.Psicologa = new Domain.Pessoa.Entities.Pessoa
            {
                Id = sessaoVM.PsicologaId,
            };
            sessao.TipoAtendimento = (Domain.Agendamento.Entities.Agendamento.tpFiltro)Convert.ToInt32(sessaoVM.TipoAtendimento);

            //sessao.Evolucao = sessaoVM.Evolucao;
            sessao.Evolucao = CriptografiaSessao.Criptografar(sessaoVM.Evolucao, requisicao[4], requisicao[3]);

            if (sessao.Validar())
            {
                operacao = _prontuarioSessao.EvoluirSessao(sessao);
                if (operacao)
                    sessaoVM.Id = sessao.Id;
            }
            //Log da operação

            if (operacao)
            {
                _logAplicacaoService.Registrar(sessaoVM.Id, requisicao, dadosExistente, sessao, "ProntuarioSessao", "ApplicationProntuarioSessaoService", "EvoluirSessao");
            }
            return (operacao, sessao.ValidationResult);
        }

        public IEnumerable<ProntuarioConsultaSessaoViewModel> Consultar(string termo, int protocoloId, int filtroTipoAtendimento, PaginacaoDados paginacao)
        {
            List<ProntuarioConsultaSessaoViewModel> retorno = new List<ProntuarioConsultaSessaoViewModel>();
            var sessoes = _prontuarioSessao.Consultar(termo, protocoloId, filtroTipoAtendimento, paginacao);
            foreach (var sessao in sessoes)
            {
                retorno.Add(FormatarConsultaViewModel(sessao));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == PaginacaoDados.TpOrdenacao.Nome)
            {
                retorno = retorno.OrderByDescending(o => o.DataSessao).ThenByDescending(o => TimeSpan.Parse(o.HoraInicio)).ToList();
            }

            return retorno;
        }

        public ProntuarioConsultaSessaoViewModel ObterSessao(int id, string[] requisicao)
        {
            var sessao = _prontuarioSessao.ObterSessao(id);
            if (Convert.ToInt32(requisicao[4]) == sessao.Psicologa.Id)
            {
                sessao.Evolucao = CriptografiaSessao.Descriptografar(sessao.Evolucao, requisicao[4], requisicao[3]);
            }
            return FormatarConsultaViewModel(sessao);
        }

        public bool ExcluirSessao(int id)
        {
            bool operacao = false;
            var dadosExistente = _prontuarioSessao.ObterSessao(id);
            operacao = _prontuarioSessao.ExcluirSessao(id);
            if (operacao)
            {
                _logAplicacaoService.Registrar(id, null, dadosExistente, null, "ProntuarioSessao", "ApplicationProntuarioSessaoService", "ExcluirSessao");
            }
            return operacao;
        }

        internal ProntuarioConsultaSessaoViewModel FormatarConsultaViewModel(Domain.ProntuarioSessao.Entities.ProntuarioSessao ps)
        {
            if (ps == null)
                return null;

            return new ProntuarioConsultaSessaoViewModel
            {
                Id = ps.Id,
                ProntuarioId = ps.Prontuario.Id,
                AgendamentoId = ps.Agendamento?.Id,
                DataSessao = ps.DataSessao,
                HoraInicio = ps.HoraInicio.ToString(@"hh\:mm"),
                HoraFim = ps.HoraFim.ToString(@"hh\:mm"),
                PsicologaId = ps.Psicologa.Id,
                PsicologaNome = ps.Psicologa.Nome,
                TipoAtendimento = (int)ps.TipoAtendimento,
                TipoAtendimentoNome = Utils.ObterDescricaoEnum(ps.TipoAtendimento),
                Evolucao = ps.Evolucao
            };
        }

        public void Dispose()
        {
        }
    }
}