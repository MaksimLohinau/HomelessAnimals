namespace HomelessAnimals.DataAccess.Entities
{
    public class Account
    {
        public int VolunteerId { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Volunteer Volunteer { get; set; }
        public bool IsEmailValid { get; set; }
        public bool HasAllowedEmailNotifications { get; set; }
        public string Salt { get; set; }
        public List<RoleAssignment> RoleAssignments { get; set; } = [];
    }
}
