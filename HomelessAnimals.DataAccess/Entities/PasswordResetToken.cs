namespace HomelessAnimals.DataAccess.Entities
{
    public class PasswordResetToken
    {
        public int AccountId { get; set; }
        public string Token { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
    }
}
