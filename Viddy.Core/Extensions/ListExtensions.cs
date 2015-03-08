using System;
using System.Collections.Generic;
using System.Linq;

namespace Viddi.Core.Extensions
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

        public static List<T> ToList<T>(this Array array)
        {
            return (from object item in array select (T)item).ToList();
        }
    }
}
