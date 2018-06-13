using System.Data.Common;
using System.Data.SqlClient;

namespace Bode.DataAccess.SqlServer
{
    public class DataAccessCommand : DataAccess.DataAccessCommand
    {
        public DataAccessCommand(string connectionString = null, string query = null) : base(connectionString, query) { }

        protected override DbCommand CreateDbCommand(string cmdText, DbConnection connection) =>  new SqlCommand(cmdText, (SqlConnection)connection);

        protected override DbConnection CreateDbConnection(string connectionString) => new SqlConnection(connectionString);

        protected override DbParameter CreateDbParameter(string name, object value) => new SqlParameter(name, value);
    }
}
