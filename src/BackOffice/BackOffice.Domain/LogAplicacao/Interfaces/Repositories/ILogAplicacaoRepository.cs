using Psicologa.Domain;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.LogAplicacao.Interfaces.Repositories
{
    public interface ILogAplicacaoRepository : IRepositoryBase<Entities.LogAplicacao>
    {
        bool Salvar(Domain.LogAplicacao.Entities.LogAplicacao log);
        IEnumerable<Entities.LogAplicacao> ObterUltimos(int top);
        Entities.LogAplicacao Obter(int id);
        IEnumerable<Domain.LogAplicacao.Entities.LogAplicacao> Consultar(string termo, PaginacaoDados paginacao);
    }
}
