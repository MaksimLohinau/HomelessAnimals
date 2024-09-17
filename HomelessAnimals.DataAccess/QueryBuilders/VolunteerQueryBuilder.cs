using HomelessAnimals.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.QueryBuilders
{
    public class VolunteerQueryBuilder
    {
        private IQueryable<Volunteer> _query;
        public VolunteerQueryBuilder(IQueryable<Volunteer> query)
        {
            _query = query;
        }

        public VolunteerQueryBuilder IncludeAnimalProfiles(bool includeAnimalProfile)
        {
            if (includeAnimalProfile)
                _query = _query.Include(x => x.Animals);

            return this;
        }

        public VolunteerQueryBuilder IncludeAccount(bool includeAccount)
        {
            if (includeAccount)
                _query = _query.Include(x => x.Account);

            return this;
        }

        public VolunteerQueryBuilder IncludeRoleAssignments(bool includeRoleAssignments)
        {
            if (includeRoleAssignments)
                _query = _query.Include(x => x.Account)
                        .ThenInclude(x => x.RoleAssignments)
                        .ThenInclude(x => x.Scopes);

            return this;
        }

        public VolunteerQueryBuilder AsNoTracking(bool asNoTracking)
        {
            if (asNoTracking)
                _query = _query.AsNoTracking();

            return this;
        }

        public IQueryable<Volunteer> Build()
        {
            return _query;
        }
    }
}
