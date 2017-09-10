using System;

namespace Bode.DataAccess.MySql
{
    public class DataAccessRepository<T> : DataAccess.DataAccessRepository<T> where T : new()
    {
        public DataAccessRepository(string connectionString) : base(connectionString, Conversions.UpperCamelToLong, "Id") { }

        public DataAccessRepository(string connectionString, Func<string, string> propToColConversion, string primaryKeyPropertyName) : base(connectionString, Conversions.LongToUpperCamel, primaryKeyPropertyName) { }

        protected override DataAccess.DataAccessCommandFactory<T> CreateCommandFactory() => new DataAccessCommandFactory<T>(ConnectionString, Table, Bindings);
    }
}
