using System;
using System.Data;

namespace Shared.Infra.Data.Providers
{
    public class ADONETUnityOfWork : IUnityOfWork
    {
        private IDbTransaction _transaction;
        private readonly Action<ADONETUnityOfWork> _rolledBack;
        private readonly Action<ADONETUnityOfWork> _committed;
        public IDbTransaction Transaction { get; private set; }

        public ADONETUnityOfWork(IDbTransaction transaction, Action<ADONETUnityOfWork> rolledBack, Action<ADONETUnityOfWork> committed)
        {
            Transaction = transaction;
            _transaction = transaction;
            _rolledBack = rolledBack;
            _committed = committed;
        }

        public void SaveChanges()
        {
            if (_transaction == null)
                throw new InvalidOperationException("May not call save changes twice.");

            _transaction.Commit();
            _committed(this);
            _transaction = null;
        }

        public void CancelChanges()
        {
            if (_transaction == null)
                throw new InvalidOperationException("May not call cancel changes twice.");

            _transaction.Rollback();
            _rolledBack(this);
            _transaction = null;
        }

        public void Dispose()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback();
            _transaction.Dispose();
            _rolledBack(this);
            _transaction = null;
        }
    }
}
