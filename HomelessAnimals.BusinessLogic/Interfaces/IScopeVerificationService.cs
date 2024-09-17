using System.Security.Claims;

namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface IScopeVerificationService
    {
        bool ValidateScopeForAnimalManage(IEnumerable<Claim> claims, int animalId);
        bool ValidateScopeForAnimalAdminReassignment(IEnumerable<Claim> claims);
        bool ValidateScopeForPhoneNumberView(IEnumerable<Claim> claims, int volunteerId);
    }
}
