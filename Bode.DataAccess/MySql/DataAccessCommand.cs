using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Bode.DataAccess.MySql
{
    public class DataAccessCommand : DataAccess.DataAccessCommand
    {
        public DataAccessCommand(string connectionString = null, string query = null) : base(connectionString, query) { }

        protected override DbCommand CreateDbCommand(string cmdText, DbConnection connection) => new MySqlCommand(cmdText, (MySqlConnection)connection);

        protected override DbConnection CreateDbConnection(string connectionString) => new MySqlConnection(connectionString);

        protected override DbParameter CreateDbParameter(string name, object value) => new MySqlParameter(name, value);
    }
}
