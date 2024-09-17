using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.QueryOptions;
using HomelessAnimals.Shared.Models;


namespace HomelessAnimals.DataAccess.Interfaces
{
    public interface IVolunteerProfileRepository
    {
        Task<Volunteer> GetVolunteer(int id, VolunteerQueryOptions queryOptions);
        void CreateVoulonteer(Volunteer volunteerProfile);
        Task<PagedResult<Volunteer>> GetVolunteers(VolounteersQueryOptions queryOptions);
        Task<bool> VolunteerAlreadyExists(string lastName);
        void DeleteVolunteer(Volunteer volunteerProfile);
    }
}
