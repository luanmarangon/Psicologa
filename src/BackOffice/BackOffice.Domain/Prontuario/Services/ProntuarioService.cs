using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Prontuario.Services
{
    public class ProntuarioService: ServiceBase<Entities.Prontuario>, IServiceBase<Entities.Prontuario>
    {
        public readonly Interfaces.Repositories.IProntuarioRepository _repository;

        public ProntuarioService(Interfaces.Repositories.IProntuarioRepository repository) 
            : base(repository)
        {
            _repository = repository;
        }



    }
}
