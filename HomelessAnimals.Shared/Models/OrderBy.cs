using HomelessAnimals.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.Shared.Models
{
    public class OrderBy
    {
        public OrderBy(string propertyName, OrderDirection orderDirection = OrderDirection.Asc)
        {
            PropertyName = propertyName;
            OrderDirection = orderDirection;
        }

        public string PropertyName { get; set; }
        public OrderDirection OrderDirection { get; set; }
    }
}
