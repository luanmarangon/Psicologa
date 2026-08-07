using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Financeiro.Interfaces.Repositories
{
    public interface IFinanceiroRepository: IRepositoryBase<Entities.Financeiro>
    {
        bool Salvar(Entities.Financeiro lancamento);
        Entities.Financeiro Obter(int id);
        IEnumerable<Entities.Financeiro> Consultar(string termo, PaginacaoDados paginacao);
        Entities.ResumoFinanceiro ObterResumo(DateTime dataInicio, DateTime dataFim);
        bool Excluir(int id);
    }
}
