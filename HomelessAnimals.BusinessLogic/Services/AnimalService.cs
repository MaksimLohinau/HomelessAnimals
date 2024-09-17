using AutoMapper;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.BusinessLogic.QueryParams;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryOptions;
using HomelessAnimals.Shared.Constants;
using HomelessAnimals.Shared.Enums;
using HomelessAnimals.Shared.Exceptions;
using System.Linq.Dynamic.Core;
using Repository = HomelessAnimals.DataAccess.Entities;

namespace HomelessAnimals.BusinessLogic.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IDataAccessFactory _dataFactory;
        private readonly IMapper _mapper;

        public AnimalService(IDataAccessFactory dataFactory, IMapper mapper)
        {
            _dataFactory = dataFactory;
            _mapper = mapper;
        }

        public async Task CreateAnimal(Animal model)
        {
            var animalProfile = _mapper.Map<Repository.Animal>(model);

            _dataFactory.AnimalProfileRepository.CreateAnimalProfile(animalProfile);

            await _dataFactory.SaveChanges();

            var options = new AccountQueryOptions { IncludeRoleAssignments = true, IncludeScopes = true };
            var animalAdminRole = await _dataFactory.AccountRepository.GetRole(RoleNames.AnimalsAdmin);

            var account = await _dataFactory.AccountRepository.GetAccount(model.VolunteerProfileId, options);
            var existingTournamentAdminRole = account.RoleAssignments.FirstOrDefault(ra => ra.RoleId == animalAdminRole.Id);

                if (existingTournamentAdminRole is null)
                {
                    account.RoleAssignments.Add(new Repository.RoleAssignment
                    {
                        RoleId = animalAdminRole.Id,
                        Scopes =
                        [
                            new Repository.Scope
                            {
                                Level = ScopeLevel.Volounteer,
                                ResourceId = animalProfile.Id
                            }
                        ]
                    });
                }
                else
                {
                    existingTournamentAdminRole.Scopes.Add(new Repository.Scope
                    {
                        Level = ScopeLevel.Volounteer,
                        ResourceId = animalProfile.Id
                    });
                }

            await _dataFactory.SaveChanges();
        }

        public async Task EditAnimal(int animalProfileId, Animal model)
        {
            var animalProfile = await _dataFactory.AnimalProfileRepository.GetAnimalProfile(animalProfileId, new AnimalQueryOptions());
            _mapper.Map(model, animalProfile);

            await _dataFactory.SaveChanges();
        }

        public async Task EditAnimalAdmins(int animalProfileId, int[] adminsIds)
        {
            var currentAdmins = await _dataFactory.AnimalProfileRepository.GetAnimalAdmins(animalProfileId);
            var currentAdminIds = currentAdmins.Select(a => a.Id).ToArray();

            var idsToAdd = adminsIds.Except(currentAdminIds).ToList();
            var idsToDelete = currentAdminIds.Except(adminsIds).ToList();

            var options = new AccountQueryOptions { IncludeRoleAssignments = true, IncludeScopes = true };
            var animalAdminRole = await _dataFactory.AccountRepository.GetRole(RoleNames.AnimalsAdmin);

            foreach (var id in idsToAdd)
            {
                var account = await _dataFactory.AccountRepository.GetAccount(id, options);

                if (account is null)
                    continue;

                var existingTournamentAdminRole = account.RoleAssignments.FirstOrDefault(ra => ra.RoleId == animalAdminRole.Id);

                if (existingTournamentAdminRole is null)
                {
                    account.RoleAssignments.Add(new Repository.RoleAssignment
                    {
                        RoleId = animalAdminRole.Id,
                        Scopes =
                        [
                            new Repository.Scope
                            {
                                Level = ScopeLevel.Volounteer,
                                ResourceId = animalProfileId
                            }
                        ]
                    });
                }
                else
                {
                    existingTournamentAdminRole.Scopes.Add(new Repository.Scope
                    {
                        Level = ScopeLevel.Volounteer,
                        ResourceId = animalProfileId
                    });
                }
            }

            foreach (var id in idsToDelete)
            {
                var account = await _dataFactory.AccountRepository.GetAccount(id, options);

                if (account is null)
                    continue;

                var existingTournamentAdminRole = account.RoleAssignments.FirstOrDefault(ra => ra.RoleId == animalAdminRole.Id);

                var scope = existingTournamentAdminRole.Scopes.FirstOrDefault(s => s.Level == ScopeLevel.Volounteer &&
                        s.ResourceId == animalProfileId);

                existingTournamentAdminRole.Scopes.Remove(scope);

                if (existingTournamentAdminRole.Scopes.Count == 0)
                {
                    account.RoleAssignments.Remove(existingTournamentAdminRole);
                }
            }

            await _dataFactory.SaveChanges();
        }

        public async Task<Animal> GetAnimal(int id)
        {
            var options = new AnimalQueryOptions { AsNoTracking = true, IncludeVolunteerProfile = true };
            var result = await _dataFactory.AnimalProfileRepository.GetAnimalProfile(id, options)
                ?? throw new NotFoundException("Tournament could not be found");

            var animalAdmins = await _dataFactory.AnimalProfileRepository.GetAnimalAdmins(id);

            var animalProfile = _mapper.Map<Animal>(result);
            animalProfile.AnimalAdmins = animalAdmins.Select(a => a.Id).ToArray();

            return animalProfile;
        }

        public async Task<PagedResult<Animal>> GetAnimals(GetAnimalsQueryParams queryParams)
        {
            var options = new AnimalsQueryOptions
            {
                AsNoTracking = true,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
            };

            var result = await _dataFactory.AnimalProfileRepository.GetPaginatedAnimals(options);

            return _mapper.Map <PagedResult< Animal>>(result);
        }
    }
}
