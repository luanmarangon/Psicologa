using Shared.Domain;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;


namespace Shared.Infra.Data.Repository
{
    public abstract class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        
        ADONETContext _dbContext;

        public ADONETContext DbContext => _dbContext;

        public RepositoryBase(IDBContextFactory dbContextFactory)
        {
            _dbContext = new ADONETContext(dbContextFactory);
        }

        internal virtual IEnumerable<TEntity> Map(IDataReader dr)
        {
            throw new NotImplementedException();
        }

        internal bool TemColuna(IDataReader dr, string colunaNome)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(colunaNome, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public virtual void Dispose()
        {
            _dbContext.Dispose();
        }

        public void NovoDbContext(ADONETContext dbContext)
        {
            _dbContext = dbContext;
        }

    }

}
 
