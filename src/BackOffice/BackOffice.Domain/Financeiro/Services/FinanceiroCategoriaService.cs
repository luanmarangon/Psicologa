using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Financeiro.Services
{
    public class FinanceiroCategoriaService : ServiceBase<Entities.FinanceiroCategoria>, IServiceBase<Entities.FinanceiroCategoria>
    {
        public readonly Interfaces.Repositories.IFinanceiroCategoriaRepository _repository;
        public FinanceiroCategoriaService(Interfaces.Repositories.IFinanceiroCategoriaRepository repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
