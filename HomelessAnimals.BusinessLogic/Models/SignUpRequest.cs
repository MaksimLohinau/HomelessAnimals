using HomelessAnimals.Shared.Enums;

namespace HomelessAnimals.BusinessLogic.Models
{
    public class SignUpRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string TelegramName { get; set; }
        public string PhoneNumber { get; set; }
        public int CityId { get; set; }
        public DateTime SubmittedOn { get; set; }
        public SignUpRequestStatus Status { get; set; }
    }
}
