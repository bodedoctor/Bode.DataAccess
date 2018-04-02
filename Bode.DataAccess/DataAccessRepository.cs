using Bode.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Bode.DataAccess
{
    public abstract class DataAccessRepository<T> where T : new()
    {
        #region Public Properties

        public string ConnectionString { get; set; }

        public string Table { get; set; }

        public BindingCollection Bindings { get; set; }

        public Func<IDataReader, T> GetFromReader { get; set; }

        #endregion

        #region Constructors

        public DataAccessRepository(string connectionString, string tableName, string primaryKeyPropertyName, Func<string, string> propToColConversion)
        {
            ConnectionString = connectionString;
            Table = propToColConversion(tableName);
            Bindings = BindingCollection.Build<T>(
                propToColConversion,
                primaryKeyPropertyName,
                new Dictionary<string, string> { { propToColConversion($"{tableName}{primaryKeyPropertyName}"), primaryKeyPropertyName } }
            );
        }

        #endregion

        #region Private Properties

        private DataAccessCommandFactory<T> _commandFactory;

        private DataAccessCommandFactory<T> commandFactory
        {
            get
            {
                if (ConnectionString == null)
                    throw new ArgumentNullException(nameof(ConnectionString));
                if (Table == null)
                    throw new ArgumentNullException(nameof(Table));
                if (Bindings == null)
                    throw new ArgumentNullException(nameof(Bindings));

                if (_commandFactory == null)
                    _commandFactory = CreateCommandFactory();
                return _commandFactory;
            }
        }

        #endregion

        public IEnumerable<T> Where(Expression<Func<T, bool>> expression)
        {
            var expressionBody = ((BinaryExpression)expression.Body);
            var left = (MemberExpression)expressionBody.Left;
            var right = expressionBody.Right;

            var propertyName = left.Member.Name;
            var value = Expression.Lambda(right).Compile().DynamicInvoke();

            return commandFactory.Where($"{Bindings.GetColumnName(propertyName)} = {value}", null).GetCollection(Build);
        }

        #region Public Methods

        public IEnumerable<T> GetAll(string orderBy = null) => commandFactory.GetAll(orderBy).GetCollection(Build);

        public T Get(int id) => commandFactory.GetByPrimaryKey(id).GetFirstOrDefault(Build);

        public IEnumerable<T> Get(params int[] ids) => commandFactory.GetByPrimaryKeys(ids).GetCollection(Build);

        public IEnumerable<T> Get(int[] ids, string orderBy = null) => commandFactory.GetByPrimaryKeys(ids).GetCollection(Build);

        public IEnumerable<T> GetBy(string field, object value, string orderBy = null) => commandFactory.GetBy(field, value, orderBy).GetCollection(Build);

        public void Delete(int id) => commandFactory.DeleteByPrimaryKey(id).ExecuteNonQuery();

        public void Delete(string condition, object parameters = null) => commandFactory.DeleteWhere(condition, parameters).ExecuteNonQuery();

        public IEnumerable<T> Where(string condition, string orderBy = null, object parameters = null) => commandFactory.Where(condition, orderBy, parameters).GetCollection(Build);

        public T FirstOrDefaultWhere(string condition, string orderBy = null, object parameters = null) => commandFactory.Where(condition, orderBy, parameters).GetFirstOrDefault(Build);

        public void Save(T obj)
        {
            obj.SetProperty(Bindings.PrimaryKey.PropertyName, commandFactory.Upsert(obj).GetScalar<int>());
        }

        #endregion

        #region Private Methods

        private T Build(IDataReader reader)
        {
            if (GetFromReader != null)
            {
                return GetFromReader(reader);
            }

            return BuildFromBindings(reader);

            throw new NotImplementedException("Custom reader not supplied!");
        }

        private T BuildFromBindings(IDataReader reader)
        {
            var newObj = new T();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var propertyName = Bindings.GetPropertyName(reader.GetName(i));
                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    var pi = newObj.GetType().GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                    {
                        var value = reader[i];
                        if (value != DBNull.Value)
                            pi.SetValue(newObj, value);
                    }
                }
            }
            return newObj;
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract DataAccessCommandFactory<T> CreateCommandFactory();

        #endregion
    }
}
