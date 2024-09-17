using HomelessAnimals.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.BusinessLogic.Models
{
    public class SignUpRequestInfoShort
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public int CityId { get; set; }
        public DateTime SubmittedOn { get; set; }
        public SignUpRequestStatus Status { get; set; }
    }
}
