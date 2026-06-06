using Psicologa.Application.Paciente.ViewsModel;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Psicologa.Application.Paciente.Services
{
    public class ApplicationPacienteService : IDisposable
    {
        private readonly IAppSettings _appSettings;
        private readonly Domain.Paciente.Services.PacienteService _pacienteService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;

        public ApplicationPacienteService(IAppSettings appSettings, Domain.Paciente.Services.PacienteService pacienteService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService)
        {
            _appSettings = appSettings;
            _pacienteService = pacienteService;
            _logAplicacaoService = logAplicacaoService;
        }

        public PacienteConsultaViewModel Obter(int id)
        {
            var paciente = _pacienteService.Obter(id);
            return FormatarRetornoConsulta(paciente);
        }

        public (bool, ValidationResult) Salvar(PacienteViewModel pacienteVM, string[] requisicao)
        {
            var dadosExistente = _pacienteService.Obter(pacienteVM.Id);

            bool operacao = false;

            Domain.Paciente.Entities.Paciente paciente = new Domain.Paciente.Entities.Paciente
            {
                Id = pacienteVM.Id,
                Ativo = pacienteVM.Ativo,
                ContatoEmergenciaNome = pacienteVM.ContatoEmergenciaNome,
                ContatoEmergenciaTelefone = pacienteVM.ContatoEmergenciaTelefone,
                DataPrimeiraSessao = pacienteVM.DataPrimeiraSessao.Date,
                Observacoes = pacienteVM.Observacoes,
                Matricula = pacienteVM.Matricula != null ? pacienteVM.Matricula : GerarMatricula(),
                Pessoa = new Domain.Pessoa.Entities.Pessoa
                {
                    Id = pacienteVM.PessoaId,
                    Nome = pacienteVM.PessoaNome
                },
                Responsavel = pacienteVM.ResponsavelId.HasValue ? new Domain.Pessoa.Entities.Pessoa
                {
                    Id = pacienteVM.ResponsavelId.Value,
                    Nome = pacienteVM.ResponsavelNome
                } : null
            };

            if(paciente.Validar())
            {
                operacao = _pacienteService.Salvar(paciente);
                if(operacao)
                {
                    pacienteVM.Id = paciente.Id;
                }
            }

            RegistrarLog(paciente.Id, requisicao, dadosExistente, "Paciente");

            return (operacao, paciente.ValidationResult);
        }


        public IEnumerable<PacienteConsultaViewModel> Consultar(string nome, PaginacaoDados paginacao, Domain.Pessoa.Entities.PessoaTipo.TpPessoa tpPessoa = Domain.Pessoa.Entities.PessoaTipo.TpPessoa.Indefinido)
        {
            List<PacienteConsultaViewModel> retorno = new List<PacienteConsultaViewModel>();

            var pessoas = _pacienteService.Consultar(nome, paginacao, tpPessoa);

            foreach (var pessoa in pessoas)
            {
                retorno.Add(FormatarRetornoConsulta(pessoa));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == Shared.Infra.CrossCutting.PaginacaoDados.TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.PessoaNome).ToList();
            }

            return retorno;
        }


        internal PacienteConsultaViewModel FormatarRetornoConsulta(Domain.Paciente.Entities.Paciente ret)
        {
            if (ret == null)
                return null;

            var retorno = new PacienteConsultaViewModel
            {
                Id = ret.Id,
                Ativo = ret.Ativo,
                ContatoEmergenciaNome = ret.ContatoEmergenciaNome,
                ContatoEmergenciaTelefone = ret.ContatoEmergenciaTelefone,
                DataPrimeiraSessao = ret.DataPrimeiraSessao.Date,
                Observacoes = ret.Observacoes,
                PessoaId = ret.Pessoa.Id,
                PessoaNome = ret.Pessoa.Nome,
                ResponsavelId = ret.Responsavel?.Id,
                ResponsavelNome = ret.Responsavel?.Nome,
                Matricula = ret.Matricula,
                ProntuarioId = ret.Prontuario != null ? ret.Prontuario.Id : 0,

            };

            return retorno;
        }


        private void RegistrarLog(int servicoId, string[] requisicao, Domain.Paciente.Entities.Paciente dadosExistente, string nomeClasse)
        {
            var dadosAtualizado = _pacienteService.Obter(servicoId);
            var (retorno, dadosAlterados) = _logAplicacaoService.ObterDiferencas(dadosExistente, dadosAtualizado);

            if (dadosAlterados.Any())
            {
                var log = _logAplicacaoService.Criar(
                    requisicao: requisicao,
                    entidade: nomeClasse,
                    entidadeId: servicoId,
                    operacao: retorno,
                    dadosAntes: dadosExistente,
                    dadosDepois: dadosAtualizado,
                    dadosAlterados: dadosAlterados,
                    aplicacao: MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    metodo: MethodBase.GetCurrentMethod()?.Name
                );
                _logAplicacaoService.Salvar(log);
            }
        }

        public string GerarMatricula()
        {
            string guid = Guid.NewGuid().ToString("N").ToUpper();

            return $"PAC{guid.Substring(0, 3)}-{guid.Substring(3, 4)}";
        }
        public void Dispose()
        {
        }
    }
}