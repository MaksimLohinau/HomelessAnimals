using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.QueryOptions;

namespace HomelessAnimals.DataAccess.Interfaces
{
    public interface IAccountRepository
    {
        void Register(Account account);
        Task<Account> GetAccount(int id, AccountQueryOptions queryOptions);
        Task<Account> GetAccount(string email, AccountQueryOptions options);
        Task<Role> GetRole(string roleName);
        Task<bool> AccountAlreadyExists(string email);
    }
}
