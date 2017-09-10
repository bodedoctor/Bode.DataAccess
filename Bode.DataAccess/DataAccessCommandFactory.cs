using Bode.DataAccess.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bode.DataAccess
{
    public abstract class DataAccessCommandFactory<T>
    {
        #region Private Members

        private readonly string _connectionString;
        private readonly string _table;
        private readonly BindingCollection _bindings;
        private readonly IDataQueryBuilder _dataQueryBuilder;

        #endregion

        #region Constructors

        public DataAccessCommandFactory(string connectionString, IDataQueryBuilder dataQueryBuilder, string table, BindingCollection bindings)
        {
            _connectionString = connectionString;
            _table = table;
            _bindings = bindings;
            _dataQueryBuilder = dataQueryBuilder;
        }

        #endregion

        #region Public Methods

        public IDataAccessCommand GetAll(string orderBy = null)
        {
            return Select();
        }

        public IDataAccessCommand GetBy(string field, object value, string orderBy = null)
        {
            var command = Where(_dataQueryBuilder.FieldEqual(field), orderBy);
            command.AddParameter(field, value);
            return command;
        }

        public IDataAccessCommand GetByPrimaryKey(int id)
        {
            return GetBy(_bindings.PrimaryKey.ColumnName, id);
        }

        public IDataAccessCommand GetByPrimaryKeys(int[] ids, string orderBy = null)
        {
            var command = CreateCommand(_connectionString);
            var paramList = new List<string>();
            for (var i = 0; i < ids.Length; i++)
            {
                paramList.Add(_dataQueryBuilder.ToParameterName($"{i}"));
                command.AddParameter($"{i}", ids[i]);
            }
            command.Query = _dataQueryBuilder.Select(_table, _dataQueryBuilder.FieldIn(_bindings.PrimaryKey.ColumnName, paramList.ToArray()), orderBy);
            return command;
        }

        public IDataAccessCommand DeleteAll()
        {
            return Delete();
        }

        public IDataAccessCommand DeleteBy(string field, object value)
        {
            var command = DeleteWhere(_dataQueryBuilder.FieldEqual(field));
            command.AddParameter(field, value);
            return command;
        }

        public IDataAccessCommand DeleteByPrimaryKey(int id)
        {
            return DeleteBy(_bindings.PrimaryKey.ColumnName, id);
        }

        public IDataAccessCommand Where(string condition, string orderBy = null, object parameters = null)
        {
            return Select(condition, orderBy, parameters);
        }

        public IDataAccessCommand DeleteWhere(string condition, object parameters = null)
        {
            return Delete(condition, parameters);
        }

        public IDataAccessCommand Upsert(T obj)
        {
            var primaryKeyValue = obj.GetPropertyValue<int>(_bindings.PrimaryKey.PropertyName);
            if (primaryKeyValue == 0)
                return Insert(obj);
            return Update(obj);
        }

        public IDataAccessCommand Insert(T obj)
        {
            var fields = _bindings.Bindings.Where(b => !b.IsPrimaryKey);
            var query = _dataQueryBuilder.Insert(_table, _bindings.Bindings.Where(b => !b.IsPrimaryKey));
            return CommandWithParameters(query, obj);
        }

        public IDataAccessCommand Update(T obj)
        {
            var fields = _bindings.Bindings.Where(b => !b.IsPrimaryKey);
            var query = _dataQueryBuilder.Update(_table, fields, _bindings.PrimaryKey);
            return CommandWithParameters(query, obj);
        }

        #endregion

        #region Private Methods

        private IDataAccessCommand CommandWithParameters(string query, T obj)
        {
            var cmd = CreateCommand(_connectionString, query);
            if (obj != null)
                cmd.AddParameters(GetObjectPropertyValues(obj));
            return cmd;
        }

        private IDataAccessCommand CommandWithParameters(string query, object parameters = null)
        {
            var cmd = CreateCommand(_connectionString, query);
            if (parameters != null)
                cmd.AddParameters(parameters);
            return cmd;
        }

        private IDataAccessCommand Select(string condition = null, string orderBy = null, object parameters = null)
        {
            return CommandWithParameters(_dataQueryBuilder.Select(_table, condition, orderBy), parameters);
        }

        private IDataAccessCommand Delete(string condition = null, object parameters = null)
        {
            return CommandWithParameters(_dataQueryBuilder.Delete(_table, condition), parameters);
        }

        private IEnumerable<KeyValuePair<string, object>> GetObjectPropertyValues(T obj)
        {
            var list = new List<KeyValuePair<string, object>>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var b in _bindings.Bindings)
            {
                var prop = properties.FirstOrDefault(p => p.Name == b.PropertyName);
                if (prop != null)
                {
                    list.Add(new KeyValuePair<string, object>(b.PropertyName, prop.GetValue(obj)));
                }
            }
            return list;
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract IDataAccessCommand CreateCommand(string connectionString = null, string query = null);

        #endregion
    }
}
