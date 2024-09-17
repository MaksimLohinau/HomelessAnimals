using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.DataAccess.QueryBuilders
{
    public class AnimalsQueryBuilder
    {
        private IQueryable<Animal> _query;

        public AnimalsQueryBuilder(IQueryable<Animal> query)
        {
            _query = query;
        }

        public AnimalsQueryBuilder AsNoTracking(bool asNoTracking)
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
