using System;


namespace Shared.Domain
{
    public abstract class ServiceBase<TEntity> : IDisposable, IServiceBase<TEntity> where TEntity : class
    {
        private readonly IRepositoryBase<TEntity> _repository;

        public ServiceBase(IRepositoryBase<TEntity> repository)
        {
            _repository = repository;
        }

        //Diferente dos outros projetos, não vamos deixar ações pré-definidas.
        //Tive a impressão que isso ingessa muito a implementação.

        //public bool Salvar(TEntity obj)
        //{
        //    return _repository.Salvar(obj);
        //}

        //public TEntity Obter(int id)
        //{
        //    return _repository.Obter(id);
        //}

        //public bool Excluir(int id)
        //{
        //    return _repository.Excluir(id);
        //}


        public void Dispose()
        {
            _repository.Dispose();
        }

    }
}
