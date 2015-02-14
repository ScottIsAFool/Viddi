using System.Collections.Generic;
using System.Linq;

namespace Viddy.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || !list.Any();
        }

        public static bool IsNullOrEmpty<T>(this Stack<T> stack)
        {
            return stack == null || !stack.Any();
        }
    }
}
