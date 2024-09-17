using HomelessAnimals.DataAccess.Entities;

namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface IPasswordHashingService
    {
        public string Encrypt(string key, out string salt);
        bool VerifyPassword(string password, string toCompare, string salt);
        PasswordResetToken GeneratePasswordResetToken(int accountId, out string token);
        string GetHashedPasswordResetToken(string token);
    }
}
