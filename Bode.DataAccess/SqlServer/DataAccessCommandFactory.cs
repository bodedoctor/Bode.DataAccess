﻿namespace Bode.DataAccess.SqlServer
{
    public class DataAccessCommandFactory<T, TPrimaryKey> : DataAccess.DataAccessCommandFactory<T, TPrimaryKey>
    {
        public DataAccessCommandFactory(string connectionString, string table, BindingCollection bindings) : base(connectionString, new SqlDataQueryBuilder(), table, bindings) { }

        protected override IDataAccessCommand CreateCommand(string connectionString = null, string query = null) => new DataAccessCommand(connectionString, query);
    }
}
