using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.DataAccess.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace HomelessAnimals.BusinessLogic.Services
{
    public class PasswordHashingService : IPasswordHashingService
        {
            private const int KeySize = 64;
            private const int Iterations = 200000;
            private readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

            public string Encrypt(string password, out string salt)
            {
                var bytes = RandomNumberGenerator.GetBytes(KeySize);
                salt = Convert.ToBase64String(bytes);

                var hash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    bytes,
                    Iterations,
                    HashAlgorithm,
                    KeySize);

                return Convert.ToBase64String(hash);
            }

            public PasswordResetToken GeneratePasswordResetToken(int accountId, out string token)
            {
                var bytes = RandomNumberGenerator.GetBytes(KeySize);

                token = Base64UrlEncoder.Encode(bytes);
                var hash = Convert.ToBase64String(SHA512.HashData(bytes));

                return new PasswordResetToken
                {
                    AccountId = accountId,
                    Token = hash,
                    ExpirationDate = DateTimeOffset.UtcNow.AddDays(1)
                };
            }

            public string GetHashedPasswordResetToken(string token)
            {
                var bytes = Base64UrlEncoder.DecodeBytes(token);

                return Convert.ToBase64String(SHA512.HashData(bytes));
            }

            public bool VerifyPassword(string password, string toCompare, string salt)
            {
                var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    Convert.FromBase64String(salt),
                    Iterations,
                    HashAlgorithm,
                    KeySize);

                return hashToCompare.SequenceEqual(Convert.FromBase64String(toCompare));
            }
    }
}
