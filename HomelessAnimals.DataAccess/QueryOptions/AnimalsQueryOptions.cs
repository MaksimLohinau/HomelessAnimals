using HomelessAnimals.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.DataAccess.QueryOptions
{
    public class AnimalsQueryOptions
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool AsNoTracking { get; set; }
    }
}
