namespace HomelessAnimals.DataAccess.QueryOptions
{
    public class AccountQueryOptions
    {
        public bool IncludeVolunteerProfile { get; set; }
        public bool IncludeRoleAssignments { get; set; }
        public bool IncludePermissions { get; set; }
        public bool IncludeScopes { get; set; }
        public bool AsNoTracking { get; set; }
        public bool IncludeRoles { get; set; }
    }
}
