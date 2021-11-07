using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheSwamp.Shared;

namespace TheSwamp.Shared
{
    public static class PropertyExtensions
    {
        /// <summary>
        /// Return the value of a property as a string value
        /// </summary>
        /// <remarks>
        /// Performs a case-insensitive search and returns the value of the specified property as a string.
        /// </remarks>
        /// <param name="properties"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public static string StringValue(this IEnumerable<IProperty> source, string name, string defaultValue = null)
        {
            string v = defaultValue;
            if (source != null && source.Any())
            {
                var p = source.SingleOrDefault(x => x.Name.EqualsNoCase(name));
                if (p != null && !string.IsNullOrEmpty(p.Value))
                {
                    v = p.Value;
                }
            }

            return v;
        }

        public static int IntValue(this IEnumerable<IProperty> source, string name, int defaultValue = 0)
        {
            return Convert.ToInt32(source.StringValue(name, defaultValue.ToString()));
        }

        public static double DoubleValue(this IEnumerable<IProperty> source, string name, double defaultValue = 0.0)
        {
            return Convert.ToDouble(source.StringValue(name, defaultValue.ToString()));
        }

        public static bool BooleanValue(this IEnumerable<IProperty> source, string name, bool defaultValue = false)
        {
            return Convert.ToBoolean(source.StringValue(name, defaultValue.ToString()));
        }
    }
}
