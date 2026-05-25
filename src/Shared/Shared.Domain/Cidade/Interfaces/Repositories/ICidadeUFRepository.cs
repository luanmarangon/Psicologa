using Shared.Domain.Cidade.Entities;
using System.Collections.Generic;

namespace Shared.Domain.Cidade.Interfaces.Repositories
{
    public interface ICidadeUFRepository : IRepositoryBase<CidadeUF>
    {
        IEnumerable<CidadeUF> ObterTodos();

    }
}
