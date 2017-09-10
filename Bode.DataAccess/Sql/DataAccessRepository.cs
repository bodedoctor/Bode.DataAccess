using System;

namespace Bode.DataAccess.Sql
{
    public class DataAccessRepository<T> : DataAccess.DataAccessRepository<T> where T : new()
    {
        public DataAccessRepository(string connectionString) : base(connectionString, (s) => s, "Id") { }

        public DataAccessRepository(string connectionString, Func<string, string> propToColConversion, string primaryKeyPropertyName) : base(connectionString, propToColConversion, primaryKeyPropertyName) { }

        protected override DataAccess.DataAccessCommandFactory<T> CreateCommandFactory() => new DataAccessCommandFactory<T>(ConnectionString, Table, Bindings);
    }
}
