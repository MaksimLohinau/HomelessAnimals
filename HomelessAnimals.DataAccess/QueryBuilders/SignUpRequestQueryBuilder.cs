using HomelessAnimals.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.DataAccess.QueryBuilders
{
    public class SignUpRequestQueryBuilder
    {
        private IQueryable<SignUpRequest> _query;

        public SignUpRequestQueryBuilder(IQueryable<SignUpRequest> query)
        {
            _query = query;
        }

        public SignUpRequestQueryBuilder IncludeCity(bool includeCity)
        {
            if (includeCity)
                _query = _query
                   .Include(s => s.City);

            return this;
        }

        public SignUpRequestQueryBuilder AsNoTracking(bool asNoTracking)
        {
            if (asNoTracking)
                _query = _query.AsNoTracking();

            return this;
        }

        public IQueryable<SignUpRequest> Build()
        {
            return _query;
        }
    }
}
