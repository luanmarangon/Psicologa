
using Psicologa.Application.Financeiro.ViewsModel;
using Psicologa.Domain.Pessoa.Services;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;


namespace Psicologa.Application.Financeiro.Services
{
    public class ApplicationFinanceiroService : IDisposable
    {
        private readonly Domain.Financeiro.Services.FinanceiroService _financeiroService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationFinanceiroService(Domain.Financeiro.Services.FinanceiroService financeiroService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _financeiroService = financeiroService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }


        public (bool, ValidationResult) Salvar(FinanceiroViewModel lancamentoVM, string[] requisicao)
        {
            var dadosExistente = _financeiroService.Obter(lancamentoVM.Id);

            bool operacao = false;
            Domain.Financeiro.Entities.Financeiro financeiro = new Domain.Financeiro.Entities.Financeiro();
            financeiro.Id = lancamentoVM.Id;
            financeiro.Descricao = lancamentoVM.Descricao;
            financeiro.Tipo = (Domain.Financeiro.Entities.Financeiro.TpTipoLancamento)lancamentoVM.Tipo;
            financeiro.Categoria = new Domain.Financeiro.Entities.FinanceiroCategoria
            {
                Id = lancamentoVM.Categoria
            };
            financeiro.Valor = lancamentoVM.Valor;
            financeiro.DataLancamento = lancamentoVM.DataLancamento;
            financeiro.Observacao = lancamentoVM.Observacao;
            financeiro.Quitado = lancamentoVM.Quitado;
            financeiro.DataQuitacao = lancamentoVM.DataQuitacao != null ? Convert.ToDateTime(lancamentoVM.DataQuitacao) : null;


            if (financeiro.Validar())
            {
                operacao = _financeiroService.Salvar(financeiro);
                if (operacao)
                {
                    lancamentoVM.Id = financeiro.Id;
                }
            }

            //RegistrarLog(financeiro.Id, requisicao, dadosExistente, "Financeiro");
            if (operacao)
            {
                _logAplicacaoService.Registrar(lancamentoVM.Id, requisicao, dadosExistente, financeiro, "Financeiro", "ApplicationFinanceiroService", "Salvar");
            }
            return (operacao, financeiro.ValidationResult);
        }


        public FinanceiroConsultaViewModel Obter(int id)
        {
            var financeiro = _financeiroService.Obter(id);
            return FormatarRetornoConsulta(financeiro);
        }

        public bool Excluir(int id, string[] requisicao)
        {
            bool operacao = false;
            var dadosExistente = _financeiroService.Obter(id);
            operacao = _financeiroService.Excluir(id);

            if (operacao)
            {
                _logAplicacaoService.Registrar(id, requisicao, dadosExistente, null, "Pessoa", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, MethodBase.GetCurrentMethod()?.Name);
            }
            return operacao;




        }

        public IEnumerable<FinanceiroConsultaViewModel> Consultar(string termo, PaginacaoDados paginacao)
        {
            List<FinanceiroConsultaViewModel> retorno = new List<FinanceiroConsultaViewModel>();

            var financeiros = _financeiroService.Consultar(termo, paginacao);

            foreach (var financeiro in financeiros)
            {
                retorno.Add(FormatarRetornoConsulta(financeiro));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == PaginacaoDados.TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(x => x.DataLancamento).ToList();
            }
            return retorno;
        }


        public ResumoFinanceiroViewModel ObterResumo(DateTime dataInicio, DateTime dataFim)
        {
            var resumo = _financeiroService.ObterResumo(dataInicio, dataFim);
            ResumoFinanceiroViewModel retorno = new ResumoFinanceiroViewModel
            {
                TotalReceitas = resumo.TotalReceita,
                TotalDespesas = resumo.TotalDespesa,
                Saldo = resumo.Saldo
            };
            return retorno;
        }

        internal FinanceiroConsultaViewModel FormatarRetornoConsulta(Domain.Financeiro.Entities.Financeiro financeiro)
        {
            if (financeiro == null)
                return null;
            FinanceiroConsultaViewModel retorno = new FinanceiroConsultaViewModel
            {
                Id = financeiro.Id,
                Tipo = (int)financeiro.Tipo,
                TipoDescricao = Utils.ObterDescricaoEnum(financeiro.Tipo),
                Descricao = financeiro.Descricao,
                Categoria = financeiro.Categoria.Id,
                CategoriaNome = financeiro.Categoria.Nome,
                Valor = financeiro.Valor,
                DataLancamento = Convert.ToDateTime(financeiro.DataLancamento),
                Observacao = financeiro.Observacao,
                Ativo = financeiro.Ativo,
                Quitado = financeiro.Quitado,
                DataQuitacao = financeiro.DataQuitacao == DateTime.MinValue ? null : financeiro.DataQuitacao,
                DataCriacao = financeiro.DataCriacao,
                DataAtualizacao = financeiro.DataAtualizacao
            };
            return retorno;
        }

        public void Dispose()
        {
        }
    }
}
