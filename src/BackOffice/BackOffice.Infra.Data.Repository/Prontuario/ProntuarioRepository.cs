using Microsoft.Extensions.Logging;
using Psicologa.Domain.Prontuario.Interfaces.Repositories;
using Shared.Infra.Data.Providers;

namespace Psicologa.Infra.Data.Repository.Prontuario
{
    public class ProntuarioRepository:RepositoryBase<Domain.Prontuario.Entities.Prontuario>, IProntuarioRepository
    {
        private readonly ILogger<ProntuarioRepository> _logger;

        public ProntuarioRepository(IDBContextFactory dbContextFactory, ILogger<ProntuarioRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }


    }
}
