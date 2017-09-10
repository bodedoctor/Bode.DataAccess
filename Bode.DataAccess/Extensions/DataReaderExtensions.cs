using System;
using System.Data;

namespace Bode.DataAccess.Extensions
{
    public static class DataReaderExtensions
    {
        public static T Value<T>(this IDataReader reader, string columnName) => GetValueOrDefault<T>(reader[columnName]);

        public static T Value<T>(this IDataReader reader, int columnIndex) => GetValueOrDefault<T>(reader[columnIndex]);

        private static T GetValueOrDefault<T>(object value)
        {
            var t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;
            return (value == DBNull.Value || value == null || (!(typeof(T).FullName == "System.String") && (value.ToString() == string.Empty)))
                ? default(T) :
                (T)Convert.ChangeType(value, t);
        }
    }
}
