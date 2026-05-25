using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace Shared.Infra.Data.Providers
{
    public class ADONETContext: IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IDBContextFactory _connectionFactory;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<ADONETUnityOfWork> _uows = new LinkedList<ADONETUnityOfWork>();

        public ADONETContext(IDBContextFactory dbContextFactory)
        {
            _connectionFactory = dbContextFactory;
            _connection = _connectionFactory.Create();
        }

        public IUnityOfWork CreateUnitOfWork()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var transaction = _connection.BeginTransaction();
            var uow = new ADONETUnityOfWork(transaction, RemoveTransaction, RemoveTransaction);
            _rwLock.EnterWriteLock();
            _uows.AddLast(uow);
            _rwLock.ExitWriteLock();
            return uow;
        }

        public IDbCommand CreateCommand()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var cmd = _connection.CreateCommand();
            _rwLock.EnterReadLock();

            if (_uows.Count > 0)
                cmd.Transaction = _uows.First.Value.Transaction;
            _rwLock.ExitReadLock();
            return cmd;
        }

        private void RemoveTransaction(ADONETUnityOfWork obj)
        {
            _rwLock.EnterWriteLock();
            _uows.Remove(obj);
            _rwLock.ExitWriteLock();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
