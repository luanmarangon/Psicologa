using Psicologa.Application.Psicologo.ViewsModel;
using Psicologa.Domain.LogAplicacao.Services;
using Psicologa.Domain.Paciente.Entities;
using Psicologa.Domain.Paciente.Services;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Linq;
using System.Reflection;

namespace Psicologa.Application.Psicologo.Services
{
    public class ApplicationPsicologoService : IDisposable
    {
        private readonly IAppSettings _appSettings;
        private readonly Domain.Psicologo.Services.PsicologoService _psicologoService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;

        public ApplicationPsicologoService(IAppSettings appSettings, Domain.Psicologo.Services.PsicologoService psicologoService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService)
        {
            _appSettings = appSettings;
            _psicologoService = psicologoService;
            _logAplicacaoService = logAplicacaoService;
        }

        public (bool, ValidationResult) Salvar(PsicologoViewModel psicologoVM, string[] requisicao)
        {
            var dadosExistente = _psicologoService.Obter(psicologoVM.Id);
            bool operacao = false;

            Domain.Psicologo.Entities.Psicologo psicologo = new Domain.Psicologo.Entities.Psicologo
            {
                Id = psicologoVM.Id,
                PessoaId = psicologoVM.PessoaId,
                Crp = psicologoVM.Crp,
                CrpUf = psicologoVM.CrpUf,
                DataEmissaoCrp = psicologoVM.DataEmissaoCrp,
                Ativo = psicologoVM.Ativo
            };

            if (psicologo.Validar())
            {
                operacao = _psicologoService.Salvar(psicologo);
                if (operacao)
                    psicologoVM.Id = psicologo.Id;
            }

            //RegistrarLog(psicologo.Id, requisicao, dadosExistente, "Psicologo");
            if (operacao)
            {
                _logAplicacaoService.Registrar(psicologoVM.Id, requisicao, dadosExistente, psicologo, "Psicologo", "ApplicationPsicologoService", "Salvar");
            }

            return (operacao, psicologo.ValidationResult);
        }

        public PsicologoConsultaViewModel Obter(int id)
        {
            var psicologo = _psicologoService.Obter(id);
            return FormatarRetornoConsulta(psicologo);
        }

        public PsicologoConsultaViewModel ObterPorPessoaId(int pessoaId)
        {
            var psicologo = _psicologoService.ObterPorPessoaId(pessoaId);
            return FormatarRetornoConsulta(psicologo);
        }

        internal PsicologoConsultaViewModel FormatarRetornoConsulta(Domain.Psicologo.Entities.Psicologo psicologo)
        {
            if (psicologo == null)
                return null;

            PsicologoConsultaViewModel ret = new PsicologoConsultaViewModel
            {
                Id = psicologo.Id,
                PessoaId = psicologo.PessoaId,
                Crp = psicologo.Crp,
                CrpUf = psicologo.CrpUf,
                DataEmissaoCrp = psicologo.DataEmissaoCrp,
                Ativo = psicologo.Ativo
            };
            return ret;
        }

        public void Dispose()
        {
        }
    }
}