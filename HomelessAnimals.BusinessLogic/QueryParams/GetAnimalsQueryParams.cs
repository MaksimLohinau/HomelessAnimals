using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.BusinessLogic.QueryParams
{
    public class GetAnimalsQueryParams
    {
        public int Page { get; set; }
        public int PageSize { get; } = 30;
    }
}
