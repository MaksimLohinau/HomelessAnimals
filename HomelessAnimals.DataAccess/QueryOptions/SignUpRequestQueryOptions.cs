using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.DataAccess.QueryOptions
{
    public class SignUpRequestQueryOptions
    {
        public bool IncludeCity { get; set; }
        public bool AsNoTracking { get; set; }
    }
}
