using Psicologa.Application.Prontuario.ViewsModel;
using Psicologa.Application.Servico.ViewsModel;
using Psicologa.Domain.Servico.Entities;
using Psicologa.Domain.Servico.Services;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Linq;
using System.Reflection;


namespace Psicologa.Application.Prontuario.Services
{
    public class ApplicationProntuarioService : IDisposable
    {
        private readonly Domain.Prontuario.Services.ProntuarioService _service;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationProntuarioService(Domain.Prontuario.Services.ProntuarioService service, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _service = service;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(ProntuarioViewModel prontuarioVM, string[] requisicao)
        {
            var dadosExistente = _service.Obter(prontuarioVM.Id);
            bool operacao = false;
            Domain.Prontuario.Entities.Prontuario prontuario = new Domain.Prontuario.Entities.Prontuario();
            prontuario.Id = prontuarioVM.Id;
            prontuario.Paciente = new Domain.Paciente.Entities.Paciente 
            { 
                Id = prontuarioVM.Paciente.Id
            };
            prontuario.QueixaPrincipal = prontuarioVM.QueixaPrincipal;
            prontuario.ObjetivoTratamento = prontuarioVM.ObjetivoTratamento;
            prontuario.HistoricoFamiliar = prontuarioVM.HistoricoFamiliar;
            prontuario.ObservacoesIniciais = prontuarioVM.ObservacoesIniciais;
            prontuario.Ativo = prontuarioVM.Ativo;

            if(prontuario.Validar())
            {
                operacao = _service.Salvar(prontuario);
                if(operacao)
                    prontuarioVM.Id = prontuario.Id;
            }

            // Log
            RegistrarLog(prontuarioVM.Id, requisicao, dadosExistente, "Servico");

            return (true, prontuario.ValidationResult);
        }
        public ProntuarioConsultaViewModel Obter(int prontuarioId)
        {
            var prontuario = _service.Obter(prontuarioId);
            return FormatarRetornoConsulta(prontuario);
        }
        public ProntuarioConsultaViewModel ObterProntuarioPorPacienteId(int pacienteId)
        {
            var prontuario = _service.ObterProntuarioPorPacienteId(pacienteId);
            return FormatarRetornoConsulta(prontuario);
        }

        internal ProntuarioConsultaViewModel FormatarRetornoConsulta(Domain.Prontuario.Entities.Prontuario pr)
        {
            if (pr == null)
                return null;

            ProntuarioConsultaViewModel vm = new ProntuarioConsultaViewModel()
            {
                Id = pr.Id,
                Paciente = new Domain.Paciente.Entities.Paciente
                {
                    Id = pr.Paciente.Id,
                    Pessoa = new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = pr.Paciente.Pessoa.Id,
                        Nome = pr.Paciente.Pessoa.Nome
                    },
                    Matricula = pr.Paciente.Matricula,
                },
                QueixaPrincipal = pr.QueixaPrincipal,
                ObjetivoTratamento = pr.ObjetivoTratamento,
                HistoricoFamiliar = pr.HistoricoFamiliar,
                ObservacoesIniciais = pr.ObservacoesIniciais,
                Ativo = pr.Ativo,
                DataCriacao = pr.DataCriacao,
                DataAtualizacao = pr.DataAtualizacao,
                DataEncerramento = pr.DataEncerramento
            };
            return vm;
        }


        private void RegistrarLog(int servicoId, string[] requisicao, Domain.Prontuario.Entities.Prontuario dadosExistente, string nomeClasse)
        {
            var dadosAtualizado = _service.Obter(servicoId);
            _logAplicacaoService.Registrar(
                entidadeId: servicoId,
                requisicao: requisicao,
                dadosAntes: dadosExistente,
                dadosDepois: dadosAtualizado,
                entidade: nomeClasse,
                aplicacao: MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                metodo: MethodBase.GetCurrentMethod()?.Name
            );
        }
        public void Dispose()
        {
        }
    }
}