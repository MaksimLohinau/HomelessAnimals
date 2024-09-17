using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryBuilders;
using HomelessAnimals.DataAccess.QueryOptions;
using HomelessAnimals.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.Repositories
{
    public class VolunteerRepository : IVolunteerProfileRepository
    {
        private readonly HomelessAnimalsContext _context;

        public VolunteerRepository(HomelessAnimalsContext context)
        {
            _context = context;
        }

        public void CreateVoulonteer(Volunteer volunteerProfile)
        {
            _context.Volunteers.Add(volunteerProfile);
        }

        public async Task<Volunteer> GetVolunteer(int id, VolunteerQueryOptions queryOptions)
        {
            var query = new VolunteerQueryBuilder(_context.Volunteers)
                .IncludeAnimalProfiles(queryOptions.IncludeAnimalProfiles)
                .IncludeAccount(queryOptions.IncludeAccount)
                .IncludeRoleAssignments(queryOptions.IncludeRoleAssignments)
                .AsNoTracking(queryOptions.AsNoTracking)
                .Build();

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<Volunteer>> GetVolunteers(VolounteersQueryOptions queryOptions)
        {
            var query = new VolunteersQueryBuilder(_context.Volunteers.AsQueryable())
                .WithAnimals(queryOptions.IsAnimalNeeds)
                .Build();

            var count = await query.CountAsync();

            var volunteers = await query.ToListAsync();

            return new PagedResult<Volunteer>(volunteers, count, queryOptions.PageSize);
        }

        private IQueryable<Volunteer> BuildGetVolunteerQuery(VolunteerQueryOptions queryOptions)
        {
            return new VolunteerQueryBuilder(_context.Volunteers)
                .IncludeAnimalProfiles(queryOptions.IncludeAnimalProfiles)
                .IncludeAccount(queryOptions.IncludeAccount)
                .IncludeRoleAssignments(queryOptions.IncludeRoleAssignments)
                .AsNoTracking(queryOptions.AsNoTracking)
                .Build();
        }

        public void DeleteVolunteer(Volunteer volunteerProfile)
        {
            var tokens = _context.PasswordResetTokens.Where(t => t.AccountId == volunteerProfile.Id);

            _context.PasswordResetTokens.RemoveRange(tokens);
            _context.Volunteers.Remove(volunteerProfile);
        }

        public async Task<bool> VolunteerAlreadyExists(string firstName)
        {
            return firstName is not null && await _context.Volunteers.AnyAsync(x => x.FirstName == firstName);
        }
    }
}
