using Psicologa.Domain.Financeiro.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;

namespace Psicologa.Domain.Financeiro.Services
{
    public class FinanceiroService : ServiceBase<Entities.Financeiro>, IServiceBase<Entities.Financeiro>
    {
        public readonly Interfaces.Repositories.IFinanceiroRepository _repository;

        public FinanceiroService(Interfaces.Repositories.IFinanceiroRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Entities.Financeiro lancamento)
        {
            if (lancamento.Id == 0)
            {
                lancamento.DataCriacao = DateTime.Now;
                lancamento.Ativo = true;
            }
            lancamento.DataAtualizacao = DateTime.Now;

            return _repository.Salvar(lancamento);
        }

        public Entities.Financeiro Obter(int id)
        {
            return _repository.Obter(id);
        }

        public bool Excluir(int id)
        {
            return _repository.Excluir(id);
        }

        public IEnumerable<Entities.Financeiro> Consultar(string termo, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");
            return _repository.Consultar(termo, paginacao);
        }

        //public Entities.ResumoFinanceiro ObterResumo(DateTime dataInicio, DateTime dataFim)
        //{
        //    return _repository.ObterResumo(dataInicio, dataFim);
        //}
        public Domain.Financeiro.Entities.ResumoFinanceiro ObterResumo(DateTime dataInicio, DateTime dataFim)
        {
            // se não veio filtro (MinValue), assume do início do mês atual até agora
            if (dataInicio == DateTime.MinValue || dataFim == DateTime.MinValue)
            {
                var hoje = DateTime.Now;
                dataInicio = new DateTime(hoje.Year, hoje.Month, 1);
                //dataFim = hoje;
                dataFim = dataInicio.AddMonths(1).AddDays(-1); // último dia do mês atual
            }
            return _repository.ObterResumo(dataInicio, dataFim);
        }
    }
}