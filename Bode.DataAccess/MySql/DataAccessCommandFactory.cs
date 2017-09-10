namespace Bode.DataAccess.MySql
{
    public class DataAccessCommandFactory<T> : DataAccess.DataAccessCommandFactory<T>
    {
        public DataAccessCommandFactory(string connectionString, string table, BindingCollection bindings) : base(connectionString, new MySqlDataQueryBuilder(), table, bindings) { }

        protected override IDataAccessCommand CreateCommand(string connectionString = null, string query = null) => new DataAccessCommand(connectionString, query);
    }
}
