using HomelessAnimals.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomelessAnimals.DataAccess.QueryBuilders
{
    public class VolunteersQueryBuilder
    {
        private IQueryable<Volunteer> _query;
        public VolunteersQueryBuilder(IQueryable<Volunteer> query)
        {
            _query = query;
        }

        public VolunteersQueryBuilder WithAnimals(bool IsAnimalsNeed)
        {
            if(IsAnimalsNeed) 
                _query = _query.Include(x => x.Animals);

            return this;
        }

        public IQueryable<Volunteer> Build()
        {
            return _query;
        }
    }
}
