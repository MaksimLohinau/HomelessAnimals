using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.BusinessLogic.QueryParams;
using HomelessAnimals.DataAccess.QueryOptions;
using System.Linq.Dynamic.Core;

namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface IAnimalService
    {
        Task CreateAnimal(Animal model);
        Task EditAnimal(int animalProfileId, Animal model);
        Task EditAnimalAdmins(int animalProfileId, int[] adminsIds);
        Task<Animal> GetAnimal(int id);
        Task<PagedResult<Animal>> GetAnimals(GetAnimalsQueryParams queryOptions);
        Task<List<Animal>> GetAnimalsByCity(int cityId);
    }
}
