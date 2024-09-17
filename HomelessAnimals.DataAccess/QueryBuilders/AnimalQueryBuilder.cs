using HomelessAnimals.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.QueryBuilders
{
    public class AnimalQueryBuilder
    {
        private IQueryable<Animal> _query;
        public AnimalQueryBuilder(IQueryable<Animal> query)
        {
            _query = query;
        }

        public AnimalQueryBuilder IncludeVolunteerProfile(bool includeVolunteerProfile)
        {
            if (includeVolunteerProfile)
                _query = _query.Include(x => x.Volunteer);

            return this;
        }

        public AnimalQueryBuilder AsNoTracking(bool asNoTracking)
        {
            if (asNoTracking)
                _query = _query.AsNoTracking();

            return this;
        }

        public IQueryable<Animal> Build()
        {
            return _query;
        }
    }
}
