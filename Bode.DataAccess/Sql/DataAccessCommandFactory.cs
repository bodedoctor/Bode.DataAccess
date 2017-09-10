namespace Bode.DataAccess.Sql
{
    public class DataAccessCommandFactory<T> : DataAccess.DataAccessCommandFactory<T>
    {
        public DataAccessCommandFactory(string connectionString, string table, BindingCollection bindings) : base(connectionString, new SqlDataQueryBuilder(), table, bindings) { }

        protected override IDataAccessCommand CreateCommand(string connectionString = null, string query = null) => new DataAccessCommand(connectionString, query);
    }
}
