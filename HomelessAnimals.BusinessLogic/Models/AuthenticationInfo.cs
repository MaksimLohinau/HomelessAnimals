namespace HomelessAnimals.BusinessLogic.Models
{
    public class AuthenticationInfo
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public string[] Permissions { get; set; }
        public Scope[] Scopes { get; set; }
    }
}
