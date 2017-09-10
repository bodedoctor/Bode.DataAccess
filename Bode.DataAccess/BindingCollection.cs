using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bode.DataAccess
{
    public class BindingCollection
    {
        public List<Binding> Bindings { get; private set; } = new List<Binding>();

        public Func<string, string> PropertyToColumnNameConverter;

        public void Add(string propertyName, string columnName)
        {
            Bindings.Add(new Binding(propertyName, columnName));
        }

        public void Add(string propertyName)
        {
            Add(propertyName, PropertyToColumnNameConverter(propertyName));
        }

        public void Remove(string propertyName, string columnName)
        {
            var binding = Bindings.FirstOrDefault(b => b.PropertyName == propertyName && b.ColumnName == columnName);
            if (binding != null)
                Bindings.Remove(binding);
        }

        public void Remove(string propertyName)
        {
            Remove(propertyName, PropertyToColumnNameConverter(propertyName));
        }

        public string GetPropertyName(string columnName)
        {
            return Bindings.FirstOrDefault(b => b.ColumnName == columnName)?.PropertyName;
        }

        public string GetColumnName(string propertyName)
        {
            return Bindings.FirstOrDefault(b => b.PropertyName == propertyName)?.ColumnName;
        }

        public Binding PrimaryKey => Bindings.SingleOrDefault(b => b.IsPrimaryKey);

        public static BindingCollection Build<T>(Func<string, string> propToCol, Expression<Func<T, int>> primaryKey, Dictionary<string, string> colToPropertBinds = null)
        {
            return Build<T>(propToCol, (primaryKey?.Body as MemberExpression)?.Member?.Name, colToPropertBinds);
        }

        public static BindingCollection Build<T>(Func<string, string> propToCol, string primaryKeyName, Dictionary<string, string> colToPropertBinds = null)
        {
            var bindings = new BindingCollection
            {
                PropertyToColumnNameConverter = propToCol
            };

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                bindings.Add(p.Name);
            }
            var pkPropName = primaryKeyName;
            bindings.Bindings.FirstOrDefault(b => b.PropertyName == pkPropName).IsPrimaryKey = true;

            // Custom bindings
            if (colToPropertBinds != null)
            {
                foreach (var customBind in colToPropertBinds)
                {
                    var binder = bindings.Bindings.FirstOrDefault(b => b.PropertyName == customBind.Value);
                    if (binder != null)
                    {
                        binder.ColumnName = customBind.Key;
                    }
                }
            }

            return bindings;
        }
    }

    public class Binding
    {
        public bool IsPrimaryKey { get; set; }

        public string PropertyName { get; set; }

        public string ColumnName { get; set; }

        public Binding(string propertyName, string columnName, bool isPrimaryKey = false)
        {
            PropertyName = propertyName;
            ColumnName = columnName;
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
