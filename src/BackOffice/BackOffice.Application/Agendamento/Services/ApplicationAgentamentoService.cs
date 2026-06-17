using Psicologa.Application.Agendamento.ViewsModel;
using Psicologa.Domain.Agendamento.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.Agendamento.Services
{
    public class ApplicationAgentamentoService : IDisposable
    {
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly Domain.Agendamento.Services.AgendamentoService _servicoAgendamento;
        private readonly Domain.Configuracao.Services.ConfiguracaoService _configuracaoService;
        private readonly Domain.Prontuario.Services.ProntuarioService _prontuarioService;
        private readonly IAppSettings _appSettings;

        public ApplicationAgentamentoService(Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService,
            Domain.Agendamento.Services.AgendamentoService servicoAgendamento,
            Domain.Configuracao.Services.ConfiguracaoService configuracaoService,
                Domain.Prontuario.Services.ProntuarioService prontuarioService,
            IAppSettings appSettings)
        {
            _logAplicacaoService = logAplicacaoService;
            _servicoAgendamento = servicoAgendamento;
            _configuracaoService = configuracaoService;
            _prontuarioService = prontuarioService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(AgendamentoViewModel dados, string[] requisicao)
        {
            var dadosExistente = _servicoAgendamento.ObterPorId(dados.Id);

            ValidationResult vr = new ValidationResult();
            bool operacao = false;

            Domain.Agendamento.Entities.Agendamento agendamento = new Domain.Agendamento.Entities.Agendamento
            {
                Id = dados.Id,
                Paciente = new Domain.Pessoa.Entities.Pessoa
                {
                    Id = dados.PacienteId
                },
                Psicologo = new Domain.Pessoa.Entities.Pessoa
                {
                    Id = dados.PsicologoId
                },
                Servico = new Domain.Servico.Entities.Servico
                {
                    Id = dados.ServicoId
                },
                DataConsulta = dados.DataConsulta,
                HoraInicio = TimeSpan.Parse(dados.HoraInicio).ToString(@"hh\:mm"),
                HoraFim = (TimeSpan.Parse(dados.HoraInicio) + TimeSpan.FromMinutes(dados.TempoSessao)).ToString(@"hh\:mm"),
                TempoSessao = dados.TempoSessao,

                Online = dados.Online,
                Presencial = !dados.Online, //dados.Presencial, //Não estou usando o Presencial pois se o Online for verdadeiro o Presencial é falso, se o Online for falso o Presencial é verdadeiro

                StatusAgendamento = (Domain.Agendamento.Entities.Agendamento.TpStatusAgendamento)dados.StatusAgendamento,
                TipoAgendamento = (Domain.Agendamento.Entities.Agendamento.TpTipoAgendamento)dados.TipoAgendamento,
                Ativo = dados.Ativo,
                ConfirmouAgendamento = dados.ConfirmouAgendamento,
                DataConfirmacao = dados.ConfirmouAgendamento ? dados.DataConfirmacao : (DateTime?)null,
            };

            if (agendamento.Validar())
                operacao = _servicoAgendamento.Salvar(agendamento);

            //if (operacao)
            //    RegistrarLog(agendamento.Id, requisicao, dadosExistente, "Agendamento");
            if (operacao)
            {
                _logAplicacaoService.Registrar(dados.Id, requisicao, dadosExistente, agendamento, "Agendamento", "ApplicationAgentamentoService", "Salvar");
            }
            return (operacao, vr);
        }
        public AgendamentoDisponibilidadeViewModel ObterDisponibilidade(int psicologoId, DateTime dataConsulta)
        {
            AgendamentoDisponibilidadeViewModel disponibilidade =
                new AgendamentoDisponibilidadeViewModel();

            disponibilidade.PsicologaId = psicologoId;
            disponibilidade.DataConsulta = dataConsulta;
            disponibilidade.HorariosDisponiveis =
                new List<AgendamentoHorariosDisponiveisViewModel>();

            var horariosAgendados =
                _servicoAgendamento.ObterDisponibilidade(psicologoId, dataConsulta);

            var configuracao =
                _configuracaoService.ObterFuncionamento();

            int diaSemana = (int)dataConsulta.DayOfWeek + 1;

            if (diaSemana == 8)
                diaSemana = 1;

            var funcionamentoDia =
                configuracao.Funcionamento
                    .FirstOrDefault(x =>
                        x.DiaSemana == diaSemana &&
                        x.Ativo);

            if (funcionamentoDia == null)
                return disponibilidade;

            // horários já ocupados
            var ocupados =
                horariosAgendados?
                    .SelectMany(x => x.HorariosAgendados)
                    .OrderBy(x => x.HoraInicio)
                    .ToList()
                ?? new List<HorarioAgendado>();

            foreach (var periodo in funcionamentoDia.Periodos)
            {
                TimeSpan inicioPeriodo =
                    TimeSpan.Parse(periodo.HoraInicio);

                TimeSpan fimPeriodo =
                    TimeSpan.Parse(periodo.HoraFim);

                // gera slots de 1h
                for (
                    TimeSpan horaAtual = inicioPeriodo;
                    horaAtual < fimPeriodo;
                    horaAtual = horaAtual.Add(TimeSpan.FromHours(1))
                )
                {
                    TimeSpan proximaHora =
                        horaAtual.Add(TimeSpan.FromHours(1));

                    // não ultrapassa o horário final
                    if (proximaHora > fimPeriodo)
                        break;

                    bool ocupado =
                        ocupados.Any(x =>
                        {
                            TimeSpan inicioOcupado =
                                TimeSpan.Parse(x.HoraInicio);

                            TimeSpan fimOcupado =
                                TimeSpan.Parse(x.HoraFim);

                            return horaAtual < fimOcupado &&
                                   proximaHora > inicioOcupado;
                        });

                    // somente horários livres
                    if (!ocupado)
                    {
                        disponibilidade.HorariosDisponiveis.Add(
                            new AgendamentoHorariosDisponiveisViewModel
                            {
                                HoraInicio =
                                    horaAtual.ToString(@"hh\:mm"),

                                HoraFim =
                                    proximaHora.ToString(@"hh\:mm")
                            });
                    }
                }
            }

            return disponibilidade;
        }
        public IEnumerable<AgendamentoConsultaViewModel> Consultar(string nome, Domain.Agendamento.Entities.Agendamento.tpFiltro filtro, PaginacaoDados paginacao)
        {
            List<AgendamentoConsultaViewModel> retorno = new List<AgendamentoConsultaViewModel>();
            var servicos = _servicoAgendamento.Consultar(nome, filtro, paginacao);

            foreach (var serv in servicos)
            {
                retorno.Add(FormatarRetornoConsulta(serv));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.DataConsulta).ToList();
            }

            return retorno;
        }
        public AgendamentoConsultaViewModel ObterPorId(int id)
        {
            var agendamento = _servicoAgendamento.ObterPorId(id);

            return FormatarRetornoConsulta(agendamento);
        }
        public AgendamentoConsultaViewModel ObterAgendamentoPorPaciente(int prontuarioId, int psicologoId, DateTime data)
        {
            var paciente = _prontuarioService.Obter(prontuarioId);
            var agendamento = _servicoAgendamento.ObterAgendamentoPorPaciente(paciente.Paciente.Pessoa.Id, psicologoId, data);
            return FormatarRetornoConsulta(agendamento);
        }
        public bool Excluir(int agendamentoId, string[] requisicao)
        {
            bool operacao = false;
            var dadosExistente = _servicoAgendamento.ObterPorId(agendamentoId);
            operacao = _servicoAgendamento.Excluir(agendamentoId);

            if (operacao)
            {
                _logAplicacaoService.Registrar(agendamentoId, requisicao, dadosExistente, null, "Agendamento", "ApplicationAgentamentoService", "Excluir");
            }

            return operacao;
        }
        internal AgendamentoConsultaViewModel FormatarRetornoConsulta(Domain.Agendamento.Entities.Agendamento agendamento)
        {
            if (agendamento == null)
                return null;

            return new AgendamentoConsultaViewModel
            {
                Id = agendamento.Id,
                PacienteId = agendamento.Paciente.Id,
                PacienteNome = agendamento.Paciente.Nome,

                PsicologoId = agendamento.Psicologo.Id,
                PsicologoNome = agendamento.Psicologo.Nome,

                ServicoId = agendamento.Servico.Id,
                ServicoNome = agendamento.Servico.Nome,

                DataConsulta = agendamento.DataConsulta,
                HoraInicio = TimeSpan.Parse(agendamento.HoraInicio).ToString(@"hh\:mm"),
                HoraFim = TimeSpan.Parse(agendamento.HoraFim).ToString(@"hh\:mm"),
                TempoSessao = agendamento.TempoSessao,
                Online = agendamento.Online,
                Presencial = agendamento.Presencial,
                DataCriacao = agendamento.DataCriacao,
                DataAtualizacao = agendamento.DataAtualizacao,
                Ativo = agendamento.Ativo,

                ConfirmouAgendamento = agendamento.ConfirmouAgendamento,
                DataConfirmacao = agendamento.DataConfirmacao != null ? agendamento.DataConfirmacao : (DateTime?)null,

                StatusAgendamentoDescricao = agendamento.StatusAgendamento.ToString(),
                TipoAgendamentoDescricao = agendamento.TipoAgendamento.ToString(),

                StatusAgendamento = (int)agendamento.StatusAgendamento,
                TipoAgendamento = (int)agendamento.TipoAgendamento
            };
        }
        public void Dispose()
        {
        }
    }
}