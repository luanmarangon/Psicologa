namespace Shared.Domain
{
    public interface IServiceBase<TEntity> where TEntity: class
    {
        //Diferente dos outros projetos, não vamos deixar ações pré-definidas.
        //Tive a impressão que isso ingessa muito a implementação.
        //bool Salvar(TEntity obj);
        //TEntity Obter(int id);
        //bool Excluir(int id);
        void Dispose();
    }
}
