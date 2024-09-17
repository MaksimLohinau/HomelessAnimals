using HomelessAnimals.BusinessLogic.Models;

namespace HomelessAnimals.Models
{
    public class AdminVolunteerEdit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TelegramName { get; set; }
        public string Email { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string PhoneNumber { get; set; }
        public int AccountId { get; set; }
        public List<Animal> AnimalProfiles { get; set; }
    }
}
