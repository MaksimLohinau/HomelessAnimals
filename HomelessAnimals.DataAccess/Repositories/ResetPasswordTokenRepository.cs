using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.Repositories
{
    public class ResetPasswordTokenRepository : IResetPasswordTokenRepository
    {
        private readonly HomelessAnimalsContext _context;

        public ResetPasswordTokenRepository(HomelessAnimalsContext context)
        {
            _context = context;
        }

        public void AddToken(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Add(token);
        }

        public async Task<PasswordResetToken> GetToken(string token)
        {
            return await _context.PasswordResetTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == token && DateTimeOffset.UtcNow <= x.ExpirationDate);
        }

        public async Task RemoveTokens(int accountId)
        {
            await _context.PasswordResetTokens
                .Where(x => x.AccountId == accountId)
                .ExecuteDeleteAsync();
        }
    }
}
