using System;
using System.Collections.Generic;
using System.Data;

namespace Bode.DataAccess
{
    public interface IDataAccessCommand
    {
        string ConnectionString { get; set; }

        string Query { get; set; }

        void AddParameter(string name, object value);

        void AddParameters(IEnumerable<KeyValuePair<string, object>> parameters);

        void AddParameters(object parameters);

        int ExecuteNonQuery();

        T GetScalar<T>() where T : struct;

        T GetFirstOrDefault<T>(Func<IDataReader, T> getFromReader = null) where T : new();

        ICollection<T> GetCollection<T>(Func<IDataReader, T> getFromReader = null) where T : new();
    }
}
