using System;
using System.Data.Common;

namespace Shared.Infra.Data.Providers
{
    public interface IDBContextFactory
    {
        public enum TpProvider
        {
            MySQL = 1,
            SQLServer = 2
        }
      
        DbConnection Create();
    }
}
