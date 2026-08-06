using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
