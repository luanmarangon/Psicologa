using Psicologa.Application.BlogPost.ViewsModel;
using Psicologa.Application.Convenio.ViewsModel;
using Psicologa.Domain.Servico.Entities;
using Psicologa.Domain.Servico.Services;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.Convenio.Services
{
    public class ApplicationConvenioService : IDisposable
    {
        private readonly Domain.Convenio.Services.ConvenioService _convenioService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationConvenioService(Domain.Convenio.Services.ConvenioService convenioService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _convenioService = convenioService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(ConvenioViewModel convenioVM, string[] requisicao)
        {
            var dadosExistente = _convenioService.Obter(convenioVM.Id);

            bool operacao = false;
            Domain.Convenio.Entities.Convenio convenio = new Domain.Convenio.Entities.Convenio();
            convenio.Id = convenioVM.Id;
            convenio.Nome = convenioVM.Nome;
            convenio.Icon = convenioVM.Icon;
            convenio.Ativo = convenioVM.Ativo;
            convenio.DestaqueHome = convenioVM.DestaqueHome;

            if (convenio.Validar())
            {
                operacao = _convenioService.Salvar(convenio);
                if (operacao)
                {
                    convenioVM.Id = convenio.Id;
                }
            }

            RegistrarLog(convenio.Id, requisicao, dadosExistente, "Convenio");

            return (operacao, convenio.ValidationResult);
        }

        public IEnumerable<ConvenioConsultaViewModel> Consultar(string nome, PaginacaoDados paginacao)
        {
            List<ConvenioConsultaViewModel> retorno = new List<ConvenioConsultaViewModel>();

            var convenios = _convenioService.Consultar(nome, paginacao);

            foreach (var convenio in convenios)
            {
                retorno.Add(FormatarRetornoConsulta(convenio));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.Nome).ToList();
            }

            return retorno;
        }

        public IEnumerable<ConvenioConsultaViewModel> ConsultarUltimos(int quantidade)
        {
            List<ConvenioConsultaViewModel> retorno = new List<ConvenioConsultaViewModel>();
            var convenios = _convenioService.ConsultarUltimos(quantidade);
            foreach (var convenio in convenios)
            {
                retorno.Add(FormatarRetornoConsulta(convenio));
            }
            return retorno;
        }
        public IEnumerable<ConvenioConsultaViewModel> ObterDestaquesHome()
        {
            List<ConvenioConsultaViewModel> retorno = new List<ConvenioConsultaViewModel>();
            var convenios = _convenioService.ObterDestaquesHome();
            foreach (var convenio in convenios)
            {
                retorno.Add(FormatarRetornoConsulta(convenio));
            }
            return retorno;
        }

        public ConvenioConsultaViewModel Obter(int convenioId)
        {
            ConvenioConsultaViewModel retorno = new ConvenioConsultaViewModel();
            retorno = FormatarRetornoConsulta(_convenioService.Obter(convenioId));
            return retorno;
        }

        public bool Excluir(int servicoId, string[] requisicao)
        {
            bool operacao = false;
            var dadosExistente = _convenioService.Obter(servicoId);
            operacao = _convenioService.Excluir(servicoId);

            RegistrarLog(servicoId, requisicao, dadosExistente, "Servico");


            return operacao;
        }

        internal ConvenioConsultaViewModel FormatarRetornoConsulta(Domain.Convenio.Entities.Convenio convenio)
        {
            var convenioViewModel = new ConvenioConsultaViewModel
            {
                Id = convenio.Id,
                Nome = convenio.Nome,
                Icon = convenio.Icon,
                DataCriacao = convenio.DataCriacao,
                DataAtualizacao = convenio.DataAtualizacao,
                Ativo = convenio.Ativo,
                DestaqueHome = convenio.DestaqueHome
            };

            return convenioViewModel;
        }

        private void RegistrarLog(int servicoId, string[] requisicao, Domain.Convenio.Entities.Convenio dadosExistente, string nomeClasse)
        {
            var dadosAtualizado = _convenioService.Obter(servicoId);
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

        public void Dispose()
        {
        }
    }
}