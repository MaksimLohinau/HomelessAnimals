using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.Models;
using Business = HomelessAnimals.BusinessLogic.Models;
using Repository = HomelessAnimals.DataAccess.Entities;
using AutoMapper;
using Animal = HomelessAnimals.Models.Animal;

namespace HomelessAnimals.Mapping
{
    public class AnimalProfile : Profile
    {
        public AnimalProfile()
        {
            CreateMap<Animal, Business.Animal>();

            CreateMap<Business.Animal, Repository.Animal>();
        }
    }
}
