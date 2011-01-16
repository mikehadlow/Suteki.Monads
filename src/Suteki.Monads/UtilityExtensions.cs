using System;
using System.Collections.Generic;

namespace Suteki.Monads
{
    public static class UtilityExtensions
    {
        public static IList<T> AddFluently<T>(this IList<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IEnumerable<T> Cons<T>(this T item, IEnumerable<T> list)
        {
            yield return item;
            foreach (var listItem in list)
            {
                yield return listItem;
            }
        }
    }
}