using Shared.Infra.Data.Providers;

namespace Shared.Domain
{
    public interface IRepositoryBase<TEntity> where TEntity: class
    {

        void NovoDbContext(ADONETContext dbContext);
        //Diferente dos outros projetos, não vamos deixar ações pré-definidas.
        //Tive a impressão que isso ingessa muito a implementação.
        //bool Salvar(TEntity obj);
        //TEntity Obter(int id);
        //bool Excluir(int id);


        void Dispose();
    }
}
