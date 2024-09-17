using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.QueryOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.DataAccess.Interfaces
{
    public interface ISignUpRequestRepository
    {
        void AddSignUpRequest(SignUpRequest signUpRequest);
        void DeleteSignUpRequest(SignUpRequest signUpRequest);
        Task<SignUpRequest> GetSignUpRequest(int id, SignUpRequestQueryOptions queryOptions);
        Task<List<SignUpRequest>> GetSignUpRequests();
        Task<bool> SignUpRequestAlreadyExists(string email);
    }
}
