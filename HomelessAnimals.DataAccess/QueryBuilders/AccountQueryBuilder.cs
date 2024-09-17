using HomelessAnimals.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.QueryBuilders
{
    public class AccountQueryBuilder
    {
        private IQueryable<Account> _query;
        public AccountQueryBuilder(IQueryable<Account> query)
        {
            _query = query;
        }

        public AccountQueryBuilder IncludeVolunteerProfile(bool includeVolunteerProfile)
        {
            if (includeVolunteerProfile)
                _query = _query.Include(x => x.Volunteer);

            return this;
        }

        public AccountQueryBuilder IncludeRoleAssignments(bool includeRoleAssignments)
        {
            if (includeRoleAssignments)
                _query = _query.Include(x => x.RoleAssignments);

            return this;
        }

        public AccountQueryBuilder IncludeRoles(bool includeRoles)
        {
            if (includeRoles)
                _query = _query.Include(x => x.RoleAssignments)
                    .ThenInclude(ra => ra.Role);

            return this;
        }

        public AccountQueryBuilder IncludePermissions(bool includePermissions)
        {
            if (includePermissions)
                _query = _query.Include(x => x.RoleAssignments)
                    .ThenInclude(ra => ra.Role)
                    .ThenInclude(r => r.Permissions);

            return this;
        }

        public AccountQueryBuilder IncludeScopes(bool includeScopes)
        {
            if (includeScopes)
                _query = _query.Include(x => x.RoleAssignments)
                    .ThenInclude(ra => ra.Scopes);

            return this;
        }

        public AccountQueryBuilder AsNoTracking(bool asNoTracking)
        {
            if (asNoTracking)
                _query = _query.AsNoTracking();

            return this;
        }

        public IQueryable<Account> Build()
        {
            return _query;
        }
    }
}
