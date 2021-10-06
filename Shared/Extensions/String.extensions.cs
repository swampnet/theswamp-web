using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Shared
{
    public static class StringExtensions
    {
        /// <summary>
        /// Perform a case insensitive comparison
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool EqualsNoCase(this string lhs, string rhs)
        {
            // Both null -> true
            if (lhs == null && rhs == null)
            {
                return true;
            }

            // One null -> false
            if (lhs == null || rhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs, StringComparison.OrdinalIgnoreCase);
        }
    }
}
