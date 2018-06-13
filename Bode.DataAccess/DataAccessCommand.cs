using Bode.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Bode.DataAccess
{
    public abstract class DataAccessCommand : IDataAccessCommand
    {
        #region Private Members

        private ICollection<DbParameter> _parameters;

        #endregion

        #region Public Properties

        public string ConnectionString { get; set; }

        public string Query { get; set; }

        #endregion

        #region Constructor

        public DataAccessCommand(string connectionString = null, string query = null)
        {
            ConnectionString = connectionString;
            Query = query;
        }

        #endregion

        #region Public Methods

        public ICollection<T> GetCollection<T>(Func<IDataReader, T> getFromReader = null) where T : new()
        {
            return ExecuteReader<ICollection<T>>((reader) =>
            {
                var result = new Collection<T>();
                while (reader.Read())
                {
                    if (getFromReader != null)
                    {
                        result.Add(getFromReader(reader));
                    }
                    else
                    {
                        result.Add(GetFromReader<T>(reader));
                    }
                }
                return result;
            });
        }

        public int ExecuteNonQuery() => OpenCommand(command => command.ExecuteNonQuery());

        public T GetFirstOrDefault<T>(Func<IDataReader, T> getFromReader = null) where T : new()
        {
            return ExecuteReader(reader =>
            {
                var result = default(T);
                if (reader.Read())
                {
                    result = getFromReader(reader);
                }
                return result;
            });
        }

        public T GetScalar<T>() where T : struct
        {
            return OpenCommand(command => ReflectionExtensions.ChangeType<T>(command.ExecuteScalar()));
        }

        public void AddParameter(string name, object value)
        {
            if (_parameters == null)
            {
                _parameters = new Collection<DbParameter>();
            }
            _parameters.Add(CreateDbParameter(name, value ?? DBNull.Value));
        }

        public void AddParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            foreach (var k in parameters)
            {
                AddParameter(k.Key, k.Value);
            }
        }

        public void AddParameters(object parameters)
        {
            if (_parameters == null)
            {
                _parameters = new Collection<DbParameter>();
            }
            foreach (var p in GetParamsFromObj(parameters))
            {
                _parameters.Add(p);
            }
        }

        #endregion

        #region Private Methods

        private T OpenCommand<T>(Func<DbCommand, T> getFromCommand)
        {
            var r = default(T);

            using (var connection = CreateDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var command = CreateDbCommand(Query, connection))
                    {
                        if (_parameters != null)
                        {
                            command.Parameters.AddRange(_parameters.ToArray());
                        }
                        r = getFromCommand(command);
                    }
                }
                finally
                {
                    connection.Close();
                }
                return r;
            }
        }

        private T ExecuteReader<T>(Func<IDataReader, T> getFromReader)
        {
            return OpenCommand(command => getFromReader(command.ExecuteReader()));
        }

        private T GetFromReader<T>(IDataReader reader) where T : new()
        {
            var newObj = new T();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var pi = newObj.GetType().GetProperty(reader.GetName(i), BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    var value = reader[i];
                    pi.SetValue(newObj, value != DBNull.Value ? value : null);
                }
            }
            return newObj;
        }

        private IEnumerable<DbParameter> GetParamsFromObj(object obj)
        {
            return obj?.GetType().GetProperties().Select(p => CreateDbParameter(p.Name, p.GetValue(obj, null)));
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract DbCommand CreateDbCommand(string cmdText, DbConnection connection);

        protected abstract DbConnection CreateDbConnection(string connectionString);

        protected abstract DbParameter CreateDbParameter(string name, object value);

        #endregion
    }
}