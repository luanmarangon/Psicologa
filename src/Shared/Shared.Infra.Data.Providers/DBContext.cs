using System;
using System.Data.Common;


namespace Shared.Infra.Data.Providers
{
    public class DBContext : IDBContextFactory
    {
        private readonly string _connectionString;
        private readonly IDBContextFactory.TpProvider _tpProvider;

        public DBContext(string connectionString, IDBContextFactory.TpProvider tpProvider)
        {
            _connectionString = connectionString;
            _tpProvider = tpProvider;
        }

        public DbConnection Create()
        {
            DbConnection connection = null;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("String de Conexão não fornecida.");
            }

            if (_tpProvider == IDBContextFactory.TpProvider.MySQL)
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection();
            }
            else if (_tpProvider == IDBContextFactory.TpProvider.SQLServer)
            {
                connection = new Microsoft.Data.SqlClient.SqlConnection();
            }

            if (connection == null)
                throw new Exception("Falhou ao criar a conexão");

            connection.ConnectionString = _connectionString;
            //connection.Open();

            return connection;
        }
    }
}
