using System;

namespace Bode.DataAccess.Sql
{
    public class DataAccessRepository<T> : DataAccess.DataAccessRepository<T> where T : new()
    {
        public DataAccessRepository(string connectionString) : base(connectionString, typeof(T).Name, "Id", (s) => s) { }

        public DataAccessRepository(string connectionString, string tableName, string primaryKeyPropertyName) : base(connectionString, tableName, primaryKeyPropertyName, (s) => s) { }

        public DataAccessRepository(string connectionString, Func<string, string> propToColConversion, string primaryKeyPropertyName) : base(connectionString, typeof(T).Name, primaryKeyPropertyName, (s) => s) { }

        protected override DataAccess.DataAccessCommandFactory<T> CreateCommandFactory() => new DataAccessCommandFactory<T>(ConnectionString, Table, Bindings);
    }
}
