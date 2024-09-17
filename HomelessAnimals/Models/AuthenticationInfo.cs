using HomelessAnimals.BusinessLogic.Models;

namespace HomelessAnimals.Models
{
    public class AuthenticationInfo
    {
        public int VolunteerId { get; set; }
        public string Name { get; set; }
        public string[] Permissions { get; set; }
        public Scope[] Scopes { get; set; }
    }
}
