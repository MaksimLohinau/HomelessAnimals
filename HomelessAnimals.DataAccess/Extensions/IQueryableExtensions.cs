using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.DataAccess.Extensions
{
    internal static class IQueryableExtensions
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            return query
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize);
        }
    }
}
