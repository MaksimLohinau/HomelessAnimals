using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.Shared.Models
{
    public class RateLimiterSettings
    {
        public int Window { get; set; }
        public int PermitLimit { get; set; }
        public int QueueLimit { get; set; }
    }
}
