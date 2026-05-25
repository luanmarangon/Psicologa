using System;
using System.Data;

namespace Shared.Infra.Data.Providers
{
    public static class CommandExtensions
    {
        public static IDbDataParameter ParameterAdd(this IDbCommand command, string name, object value)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (name == null)
                throw new ArgumentNullException("name");

            var p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            command.Parameters.Add(p);

            return p;
        }

        public static IDbDataParameter ParameterAdd(this IDbCommand command, string name, object value, DbType type)
        {
            var p = ParameterAdd(command, name, value);
            p.DbType = type;
            return p;
        }

        public static IDbDataParameter ParameterAdd(this IDbCommand command, string name, object value, ParameterDirection direction)
        {
            var p = ParameterAdd(command, name, value);
            p.Direction = direction;
            return p;
        }

        public static IDbDataParameter ParameterAdd(this IDbCommand command, string name, object value, DbType type, ParameterDirection direction)
        {
            var p = ParameterAdd(command, name, value);
            p.DbType = type;
            p.Direction = direction;
            return p;
        }

        public static object ParameterValue(this IDbCommand command, string name)
        {
            return ((IDbDataParameter)command.Parameters[name]).Value;

        }

        public static void ParametersClear(this IDbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            command.Parameters.Clear();
        }
    }
}
