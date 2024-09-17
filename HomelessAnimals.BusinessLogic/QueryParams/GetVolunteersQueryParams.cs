using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.BusinessLogic.QueryParams
{
    public class GetVolunteersQueryParams
    {
        public int? Page { get; set; }
        public string[] Country { get; set; }
        public string Search { get; set; }
        public int PageSize { get; } = 20;
    }
}
