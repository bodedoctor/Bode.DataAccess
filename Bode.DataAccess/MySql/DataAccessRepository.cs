using System;

namespace Bode.DataAccess.MySql
{
    public class DataAccessRepository<T, TPrimaryKey> : DataAccess.DataAccessRepository<T, TPrimaryKey> where T : new() where TPrimaryKey : struct
    {
        public DataAccessRepository(string connectionString) : base(connectionString, typeof(T).Name, "Id", Conversions.UpperCamelToLong) { }

        public DataAccessRepository(string connectionString, string tableName, string primaryKeyPropertyName) : base(connectionString, tableName, primaryKeyPropertyName, Conversions.LongToUpperCamel) { }

        public DataAccessRepository(string connectionString, Func<string, string> propToColConversion, string primaryKeyPropertyName) : base(connectionString, typeof(T).Name, primaryKeyPropertyName, Conversions.LongToUpperCamel) { }

        protected override DataAccess.DataAccessCommandFactory<T, TPrimaryKey> CreateCommandFactory() => new DataAccessCommandFactory<T, TPrimaryKey>(ConnectionString, Table, Bindings);
    }
}
