using System;
using System.Collections.Generic;
using System.Linq;
using Viddy.ViewModel;

namespace Viddy.Extensions
{
    public static class ListTypeExtensions
    {
        public static List<IListType> AddEveryOften(this List<IListType> list, int everyOften, int maxNumberToShow, IListType itemToAdd) 
        {
            if (list.IsNullOrEmpty())
            {
                return new List<IListType>();
            }

            var itemsAdded = 0;
            var numberOfItemsToAdd = Math.Floor((decimal)list.Count / everyOften);

            for (var i = 0; i < numberOfItemsToAdd; i++)
            {
                var addAt = (i + 1) * everyOften;
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
