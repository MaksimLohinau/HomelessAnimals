namespace HomelessAnimals.DataAccess.Entities
{
    public class Volunteer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string TelegramName { get; set; }
        public Account? Account { get; set; }
        public int AccountId { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public string PhoneNumber { get; set; }
        public List<Animal> Animals { get; set; } = new();
    }
}
