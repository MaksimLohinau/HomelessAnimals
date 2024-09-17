using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.BusinessLogic.QueryParams;
using HomelessAnimals.DataAccess.QueryOptions;

namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface IVolunteerService
    {
        Task<Volunteer> GetVolunteerProfile(int id);
        Task EditVolunteerProfile(Volunteer model);
        Task EditVolunteer(Volunteer model);
        Task<List<Volunteer>> GetVolunteerProfiles(GetVolunteersQueryParams queryParams);
        Task<string> GetVolunteerEmail(int id);
        Task<string> GetVolunteerPhoneNumber(int id);
        Task<List<Models.Animal>> GetVolunteerAnimals(int id);
        Task DeleteVolunteerProfile(int id);
    }
}
