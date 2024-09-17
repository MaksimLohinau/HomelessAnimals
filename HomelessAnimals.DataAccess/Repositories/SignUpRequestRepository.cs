using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryBuilders;
using HomelessAnimals.DataAccess.QueryOptions;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.Repositories
{
    public class SignUpRequestRepository : ISignUpRequestRepository
    {
        private readonly HomelessAnimalsContext _context;

        public SignUpRequestRepository(HomelessAnimalsContext context)
        {
            _context = context;
        }

        public void AddSignUpRequest(SignUpRequest signUpRequest)
        {
            _context.SignUpRequests.Add(signUpRequest);
        }

        public void DeleteSignUpRequest(SignUpRequest signUpRequest)
        {
            _context.SignUpRequests.Remove(signUpRequest);
        }

        public async Task<SignUpRequest> GetSignUpRequest(int id, SignUpRequestQueryOptions queryOptions)
        {
            var query = new SignUpRequestQueryBuilder(_context.SignUpRequests.AsQueryable())
                .IncludeCity(queryOptions.IncludeCity)
                .AsNoTracking(queryOptions.AsNoTracking)
                .Build();

            return await query.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SignUpRequest>> GetSignUpRequests()
        {
            return await _context.SignUpRequests
                .OrderByDescending(s => s.SubmittedOn)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> SignUpRequestAlreadyExists(string email)
        {
            return await _context.SignUpRequests
                .AnyAsync(s => s.Email == email);
        }
    }
}
