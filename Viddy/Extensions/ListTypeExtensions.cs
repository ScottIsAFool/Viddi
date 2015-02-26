using System;
using System.Collections.Generic;
using System.Linq;
using Viddy.Core.Extensions;
using Viddy.Model;

namespace Viddy.Extensions
{
    public static class ListTypeExtensions
    {
        public static List<IListType> AddEveryOften(this IEnumerable<IListType> collectionList, int everyOften, int maxNumberToShow, IListType itemToAdd, int startAt = 0, bool addAsFirstItem = false)
        {
            var list = collectionList.ToList();
            if (list.IsNullOrEmpty())
            {
                return new List<IListType>();
            }

            var itemsAdded = 0;
            var numberOfItemsToAdd = Math.Floor((decimal)list.Count / everyOften);

            for (var i = 0; i < numberOfItemsToAdd; i++)
            {
                var multiplyValue = addAsFirstItem && startAt > 0 ? i : i + 1;
                var addAt = addAsFirstItem && i == 0 ? startAt : (multiplyValue * everyOften) + startAt;
                if (addAt > list.Count)
                {
                    break;
                }

                list.Insert(addAt, itemToAdd);

                itemsAdded++;
                if (itemsAdded == maxNumberToShow)
                {
                    break;
                }
            }

            return list;
        }
    }
}
