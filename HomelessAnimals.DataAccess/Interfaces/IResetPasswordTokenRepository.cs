using HomelessAnimals.DataAccess.Entities;

namespace HomelessAnimals.DataAccess.Interfaces
{
    public interface IResetPasswordTokenRepository
    {
        void AddToken(PasswordResetToken token);
        Task<PasswordResetToken> GetToken(string token);
        Task RemoveTokens(int accountId);
    }
}
