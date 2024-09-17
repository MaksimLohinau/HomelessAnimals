using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.Shared.Enums;

namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface ISignUpRequestService
    {
        Task SubmitSignUpRequest(SignUpRequest model);
        Task<SignUpRequest> GetSignUpRequestInfo(int id);
        Task<List<SignUpRequestInfoShort>> GetSignUpRequests();
        Task ChangeSignUpRequestStatus(int id, SignUpRequestStatus status);
    }
}
