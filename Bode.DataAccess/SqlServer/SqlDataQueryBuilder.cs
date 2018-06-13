using System.Collections.Generic;
using System.Linq;

namespace Bode.DataAccess.SqlServer
{
    public class SqlDataQueryBuilder : IDataQueryBuilder
    {
        public string Select(string table, string condition = null, string orderBy = null)
        {
            return $"SELECT * FROM [{table}]{WhereIfNotNull(condition)}{OrderByIfNotNull(orderBy)}";
        }

        public string Insert(string table, IEnumerable<Binding> insertFields, Binding primaryKey)
        {
            return $"INSERT INTO [{table}] ({string.Join(",", insertFields.Select(f => $"[{f.ColumnName}]"))}) OUTPUT INSERTED.[{primaryKey.ColumnName}] VALUES ({string.Join(",", insertFields.Select(f => $"{ToParameterName(f.PropertyName)}"))})";
        }

        public string Update(string table, IEnumerable<Binding> updateFields, Binding primaryKey)
        {
            return $"UPDATE [{table}] SET {string.Join(",", updateFields.Select(f => $"[{f.ColumnName}]={ToParameterName(f.PropertyName)}"))} WHERE {primaryKey.ColumnName}={ToParameterName(primaryKey.PropertyName)} SELECT {ToParameterName(primaryKey.PropertyName)}";
        }

        public string Delete(string table, string condition = null)
        {
            return $"DELETE FROM [{table}]{WhereIfNotNull(condition)}";
        }

        public string WhereIfNotNull(string condition)
        {
            return !string.IsNullOrWhiteSpace(condition) ? $" WHERE {condition}" : string.Empty;
        }

        public string OrderByIfNotNull(string orderBy)
        {
            return !string.IsNullOrWhiteSpace(orderBy) ? $" ORDER BY {orderBy}" : string.Empty;
        }

        public string ToParameterName(string name)
        {
            return $"@{name}";
        }

        public string FieldEqual(string name)
        {
            return $"[{name}]={ToParameterName(name)}";
        }

        public string FieldIn(string name, string[] parameters)
        {
            return $"[{name}] IN ({string.Join(",", parameters)})";
        }
    }
}
