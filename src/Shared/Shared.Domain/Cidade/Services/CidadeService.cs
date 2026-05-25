using Shared.Domain.Cidade.Interfaces.Repositories;
using System.Collections.Generic;

namespace Shared.Domain.Cidade.Services
{
    public class CidadeService : ServiceBase<Domain.Cidade.Entities.Cidade>, IServiceBase<Domain.Cidade.Entities.Cidade>
    {
        public readonly ICidadeRepository _repository;

        public CidadeService(ICidadeRepository repository)
            :base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<Entities.Cidade> Obter(string UF)
        {
            return _repository.Obter(UF);
        }

        public Entities.Cidade ObterPorIBGECodigo(int ibge)
        {
            return _repository.ObterPorIBGECodigo(ibge);
        }

        public Entities.Cidade ObterPorNome(string nome)
        {
            return _repository.ObterPorNome(nome);
        }

        public Entities.Cidade ObterPorNomeUf(string nome, string uf)
        {
            return _repository.ObterPorNomeUf(nome, uf);
        }

        public IEnumerable<Entities.Cidade> ObterTodas()
        {
            return _repository.ObterTodas();
        }
    }
}
