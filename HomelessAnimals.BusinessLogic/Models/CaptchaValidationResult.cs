using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.BusinessLogic.Models
{
    public class CaptchaValidationResult
    {
        public bool Success { get; set; }
        public double Score { get; set; }
        public string Action { get; set; }
    }
}
