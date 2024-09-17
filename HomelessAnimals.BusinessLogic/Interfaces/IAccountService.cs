using HomelessAnimals.BusinessLogic.Models;

namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface IAccountService
    {
        Task<AuthenticationInfo> Authenticate(string email, string password);
        Task<PermissionsInfo> GetPermissions(int id);
        Task ResetPassword(string email, string templateName);
        Task SetPassword(string password, string token);
    }
}
