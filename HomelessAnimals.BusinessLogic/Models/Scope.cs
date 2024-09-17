using HomelessAnimals.Shared.Enums;

namespace HomelessAnimals.BusinessLogic.Models
{
    public class Scope
    {
        public ScopeLevel Level { get; set; }
        public int? ResourceId { get; set; }

        public override string ToString()
        {
            return $"{Level}/{ResourceId ?? 0}";
        }
    }
}
