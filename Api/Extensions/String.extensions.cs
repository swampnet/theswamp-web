using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSwamp.Api
{
    internal static class String
    {
        public static string Value(this string source)
        {
            return source?
                .Replace("NA", "", StringComparison.OrdinalIgnoreCase)
                ;
        }
    }
}
