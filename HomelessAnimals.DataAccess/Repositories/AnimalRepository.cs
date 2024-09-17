using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.Extensions;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryBuilders;
using HomelessAnimals.DataAccess.QueryOptions;
using HomelessAnimals.Shared.Enums;

using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace HomelessAnimals.DataAccess.Repositories
{
    public class AnimalRepository : IAnimalProfileRepository
    {
        private readonly HomelessAnimalsContext _context;

        public AnimalRepository(HomelessAnimalsContext context)
        {
            _context = context;
        }

        public void CreateAnimalProfile(Animal animalProfile)
        {
            _context.Animals.Add(animalProfile);
        }

        public async Task<Animal> GetAnimalProfile(int id, AnimalQueryOptions queryOptions)
        {
            var query = new AnimalQueryBuilder(_context.Animals)
                .IncludeVolunteerProfile(queryOptions.IncludeVolunteerProfile)
                .IncludeCity(queryOptions.IncludeCity)
                .AsNoTracking(queryOptions.AsNoTracking)
                .Build();

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Animal>> GetAnimals(AnimalQueryOptions queryOptions)
        {
            return await BuildGetAnimalProfileQuery(queryOptions).ToListAsync();
        }

        public async Task<List<Animal>> GetAnimalsByCity(int cityId,AnimalQueryOptions queryOptions)
        {
            return  await BuildGetAnimalProfileQuery(queryOptions).Where(x => x.CityId == cityId).ToListAsync();
        }

        public async Task<Shared.Models.PagedResult<Animal>> GetPaginatedAnimals(AnimalsQueryOptions queryOptions)
        {
            IQueryable<Animal> query = BuildGetAnimalsQuery(queryOptions);

            var count = await query.CountAsync();

            var animals = await query
                .ApplyPagination(queryOptions.Page, queryOptions.PageSize)
                .ToListAsync();

            return new Shared.Models.PagedResult<Animal>(animals, count, queryOptions.PageSize);
        }

        public async Task<List<Volunteer>> GetAnimalAdmins(int animalProfiletId)
        {
            var adminIds = await _context.RoleAssignments
                .Where(ra => ra.Scopes.Any(s => s.Level == ScopeLevel.Volounteer && s.ResourceId == animalProfiletId))
                .AsNoTracking()
                .Select(x => x.AccountId)
                .ToListAsync();

            return await _context.Volunteers
                .Where(p => adminIds.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        private IQueryable<Animal> BuildGetAnimalProfileQuery(AnimalQueryOptions queryOptions)
        {
            return new AnimalQueryBuilder(_context.Animals)
                .IncludeVolunteerProfile(queryOptions.IncludeVolunteerProfile)
                .AsNoTracking(queryOptions.AsNoTracking)
                .Build();
        }

        private IQueryable<Animal> BuildGetAnimalsQuery(AnimalsQueryOptions queryOptions)
        {
            return new AnimalsQueryBuilder(_context.Animals)
                .AsNoTracking(queryOptions.AsNoTracking)
                .Build();
        }
    }
}
