using AutoMapper;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.BusinessLogic.QueryParams;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryOptions;
using HomelessAnimals.Shared.Constants;
using HomelessAnimals.Shared.Enums;
using HomelessAnimals.Shared.Exceptions;
using Repository = HomelessAnimals.DataAccess.Entities;

namespace HomelessAnimals.BusinessLogic.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IDataAccessFactory _dataFactory;
        private readonly IMapper _mapper;
        private readonly IPasswordHashingService _passwordHashingService;

        public VolunteerService(IDataAccessFactory dataFactory, IPasswordHashingService passwordHashingService, IMapper mapper)
        {
            _dataFactory = dataFactory;
            _mapper = mapper;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<Volunteer> GetVolunteerProfile(int id)
        {
            var queryOptions = new VolunteerQueryOptions
            {
                IncludeAccount = true,
                AsNoTracking = true
            };

            var volunteerProfile = await _dataFactory.VolunteerProfileRepository.GetVolunteer(id, queryOptions)
                ?? throw new NotFoundException("Volunteer you are looking for could not be found");

            var model = _mapper.Map<Volunteer>(volunteerProfile);
            _mapper.Map(volunteerProfile.Account, model);

            return model;
        }

        public async Task EditVolunteerProfile(Volunteer model)
        {
            var queryOptions = new VolunteerQueryOptions { IncludeAccount = true, AsNoTracking = false };

            var volunteerProfile = await _dataFactory.VolunteerProfileRepository.GetVolunteer(model.Id, queryOptions)
                ?? throw new NotFoundException("Volunteer you are trying to edit could not be found");

            if (model.Email != volunteerProfile.Account?.Email)
            {
                var emailExists = await _dataFactory.AccountRepository.AccountAlreadyExists(model.Email);

                if (emailExists)
                    throw new AccountAlreadyExistsException("An account with this email already exists");
            }

            if (model.LastName != volunteerProfile.LastName)
            {
                var nicknameExists = await _dataFactory.VolunteerProfileRepository.VolunteerAlreadyExists(model.LastName);

                if (nicknameExists)
                    throw new AccountAlreadyExistsException("An account with this Playdek name already exists");
            }

            _mapper.Map(model, volunteerProfile);

            // for legacy users without existing account
            if (volunteerProfile.Account is null)
            {
                var account = new Repository.Account
                {
                    Email = model.Email,
                    Password = _passwordHashingService.Encrypt(Guid.NewGuid().ToString(), out var salt),
                    IsEmailValid = true,
                    HasAllowedEmailNotifications = true,
                    Salt = salt
                };

                var userRole = await _dataFactory.AccountRepository.GetRole(RoleNames.User);

                account.RoleAssignments.Add(new Repository.RoleAssignment
                {
                    Role = userRole,
                    Scopes = [ new Repository.Scope
                    {
                        Level = ScopeLevel.Volounteer,
                        ResourceId = account.VolunteerId,
                    }
                ]
                });

                volunteerProfile.Account = account;
            }
            else
            {
                _mapper.Map(model, volunteerProfile.Account);
            }

            await _dataFactory.SaveChanges();
        }

        public async Task EditVolunteer(Volunteer model)
        {
            var queryOptions = new VolunteerQueryOptions();

            var volunteerProfile = await _dataFactory.VolunteerProfileRepository.GetVolunteer(model.Id, queryOptions)
                ?? throw new NotFoundException("Player you are trying to edit could not be found");

            if (model.LastName != volunteerProfile.LastName)
            {
                var nicknameExists = await _dataFactory.VolunteerProfileRepository.VolunteerAlreadyExists(model.LastName);

                if (nicknameExists)
                    throw new AccountAlreadyExistsException("An account with this Playdek name already exists");
            }

            _mapper.Map(model, volunteerProfile);

            await _dataFactory.SaveChanges();
        }

        public async Task<List<Volunteer>> GetVolunteerProfiles(GetVolunteersQueryParams queryParams)
        {
            var options = _mapper.Map<VolounteersQueryOptions>(queryParams);
            var volunters = await _dataFactory.VolunteerProfileRepository.GetVolunteers(options);

            return _mapper.Map<List<Volunteer>>(volunters);
        }

        public async Task<string> GetVolunteerEmail(int id)
        {
            var queryOptions = new VolunteerQueryOptions
            {
                IncludeAccount = true,
                AsNoTracking = true
            };

            var volunteerProfile = await _dataFactory.VolunteerProfileRepository.GetVolunteer(id, queryOptions)
                ?? throw new NotFoundException("Player could not be found");

            return volunteerProfile.Account?.Email;
        }

        public async Task<string> GetVolunteerPhoneNumber(int id)
        {
            var queryOptions = new VolunteerQueryOptions
            {
                AsNoTracking = true
            };

            var volunteerProfile = await _dataFactory.VolunteerProfileRepository.GetVolunteer(id, queryOptions)
                ?? throw new NotFoundException("Player could not be found");

            return volunteerProfile.TelegramName;
        }

        public async Task<List<Models.Animal>> GetVolunteerAnimals(int id)
        {
            var options = new VolunteerQueryOptions
            {
                AsNoTracking = true,
                IncludeAnimalProfiles = true,
            };

            var volunteerProfile = await _dataFactory.VolunteerProfileRepository.GetVolunteer(id, options)
                ?? throw new NotFoundException("Player profile could not be found");

            return _mapper.Map<List<Models.Animal>>(volunteerProfile.Animals);
        }

        public async Task DeleteVolunteerProfile(int id)
        {
            var options = new VolunteerQueryOptions
            {
                IncludeAnimalProfiles = false,
                IncludeAccount = true,
                IncludeRoleAssignments = true
            };

            var volunteerProfile = await _dataFactory.VolunteerProfileRepository.GetVolunteer(id, options)
                ?? throw new NotFoundException("Player could not be found");

            _dataFactory.VolunteerProfileRepository.DeleteVolunteer(volunteerProfile);
            await _dataFactory.SaveChanges();
        }
    }
}
