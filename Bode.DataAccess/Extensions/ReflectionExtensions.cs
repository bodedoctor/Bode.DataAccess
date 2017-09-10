using System;
using System.Reflection;

namespace Bode.DataAccess.Extensions
{
    public static class ReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static void SetProperty(this object obj, string name, object value)
        {
            var info = obj?.GetType().GetProperty(name, BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (info != null)
            {
                info.SetValue(obj, value, null);
            }
        }

        public static T ChangeType<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
