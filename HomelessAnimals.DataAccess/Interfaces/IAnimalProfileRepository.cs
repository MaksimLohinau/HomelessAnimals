using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.QueryOptions;
using System.Linq.Dynamic.Core;

namespace HomelessAnimals.DataAccess.Interfaces
{
    public interface IAnimalProfileRepository
    {
        Task<Animal> GetAnimalProfile(int id, AnimalQueryOptions queryOptions);
        void CreateAnimalProfile(Animal animalProfile);
        Task<List<Animal>> GetAnimals(AnimalQueryOptions queryOptions);
        Task<List<Volunteer>> GetAnimalAdmins(int animalProfiletId);
        Task<Shared.Models.PagedResult<Animal>> GetPaginatedAnimals(AnimalsQueryOptions queryOptions);
        Task<List<Animal>> GetAnimalsByCity(int cityId, AnimalQueryOptions queryOptions);
    }
}
