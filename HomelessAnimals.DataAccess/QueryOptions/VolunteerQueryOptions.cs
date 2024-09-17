namespace HomelessAnimals.DataAccess.QueryOptions
{
    public class VolunteerQueryOptions
    {
        public bool IncludeAccount { get; set; }
        public bool IncludeAnimalProfiles { get; set; }
        public bool AsNoTracking { get; set; }
        public bool IncludeRoleAssignments { get; set; }
    }
}
