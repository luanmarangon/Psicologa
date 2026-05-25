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
    }
}
