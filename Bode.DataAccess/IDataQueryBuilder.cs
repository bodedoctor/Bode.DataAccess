using System.Collections.Generic;

namespace Bode.DataAccess
{
    public interface IDataQueryBuilder
    {
        string Select(string table, string condition = null, string orderBy = null);

        string Insert(string table, IEnumerable<Binding> insertFields);

        string Update(string table, IEnumerable<Binding> updateFields, Binding primaryKey);

        string Delete(string table, string condition = null);

        string WhereIfNotNull(string condition);

        string OrderByIfNotNull(string orderBy);

        string ToParameterName(string name);

        string FieldEqual(string name);

        string FieldIn(string name, string[] parameters);
    }
}
