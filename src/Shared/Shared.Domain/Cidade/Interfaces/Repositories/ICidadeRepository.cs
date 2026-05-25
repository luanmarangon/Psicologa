using System.Collections.Generic;

namespace Shared.Domain.Cidade.Interfaces.Repositories
{
    public interface ICidadeRepository : IRepositoryBase<Entities.Cidade>
    {
        IEnumerable<Entities.Cidade> Obter(string UF);
        Entities.Cidade ObterPorNome(string nome);
        Entities.Cidade ObterPorIBGECodigo(int ibge);
        IEnumerable<Entities.Cidade> ObterTodas();
        Entities.Cidade ObterPorNomeUf(string nome, string uf);

    }
}
