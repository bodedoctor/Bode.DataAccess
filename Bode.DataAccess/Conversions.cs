using System.Linq;
using System.Text.RegularExpressions;

namespace Bode.DataAccess
{
    public class Conversions
    {
        /// <summary>
        /// Converts "ThisIsAnExample" to "this_is_an_example"
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Converted column name</returns>
        public static string UpperCamelToLong(string propertyName)
        {
            return ToLowercaseNamingConvention(propertyName, true).Replace(' ', '_');
        }

        /// <summary>
        /// Converts "this_is_an_example" to "ThisIsAnExample"
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string LongToUpperCamel(string columnName)
        {
            var bits = columnName.Split('_');
            var titles = bits.Select(b => $"{b[0].ToString().ToUpper()}{b.Substring(1).ToLower()}");
            return string.Concat(titles);
        }

        private static string ToLowercaseNamingConvention(string s, bool toLowercase)
        {
            if (toLowercase)
            {
                var r = new Regex(@"
                 (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

                return r.Replace(s, "_").ToLower();
            }
            return s;
        }
    }
}
