using System;
using System.Collections.Generic;
using System.Text;

namespace KrisG.Utility.Extensions
{
    public static class EnumerableExtensions
    {
        public static string JoinStrings(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}
