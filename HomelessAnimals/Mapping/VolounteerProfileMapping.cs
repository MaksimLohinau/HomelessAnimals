using AutoMapper;
using Bussines =  HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.DataAccess.Entities;

namespace HomelessAnimals.Mapping
{
    public class VolounteerProfileMapping : Profile
    {
        public VolounteerProfileMapping()
        {
            CreateMap<Bussines.Volunteer, Volunteer>();
        }
    }
}
