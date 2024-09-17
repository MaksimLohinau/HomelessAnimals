namespace HomelessAnimals.DataAccess.Entities
{
    public class RoleAssignment
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public List<Scope> Scopes { get; set; } = [];
    }
}
