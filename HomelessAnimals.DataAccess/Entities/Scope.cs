using HomelessAnimals.Shared.Enums;

namespace HomelessAnimals.DataAccess.Entities
{
    public class Scope
    {
        public int Id { get; set; }
        public int RoleAssignmentId { get; set; }
        public ScopeLevel Level { get; set; }
        public int? ResourceId { get; set; }
    }
}
