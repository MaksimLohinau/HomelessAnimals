using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryBuilders;
using HomelessAnimals.DataAccess.QueryOptions;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly HomelessAnimalsContext _context;

        public AccountRepository(HomelessAnimalsContext context)
        {
            _context = context;
        }
        public void Register(Account account)
        {
            _context.Accounts.Add(account);
        }
        public async Task<Account> GetAccount(int id, AccountQueryOptions queryOptions)
        {
            var query = GetAccountBaseQuery(queryOptions);

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Account> GetAccount(string email, AccountQueryOptions options)
        {
            var query = GetAccountBaseQuery(options);

            return await query
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Email == email);
        }
        public async Task<Role> GetRole(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }
        public async Task<bool> AccountAlreadyExists(string email)
        {
            return await _context.Accounts.AnyAsync(x => x.Email == email);
        }

        private IQueryable<Account> GetAccountBaseQuery(AccountQueryOptions queryOptions)
        {
            return new AccountQueryBuilder(_context.Accounts.AsQueryable())
                .IncludeRoles(queryOptions.IncludeRoles)
                .IncludeVolunteerProfile(queryOptions.IncludeVolunteerProfile)
                .AsNoTracking(queryOptions.AsNoTracking)
                .Build();
        }
    }
}
