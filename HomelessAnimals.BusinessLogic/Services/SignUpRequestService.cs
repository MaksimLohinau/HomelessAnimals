using AutoMapper;
using HomelessAnimals.BusinessLogic.EmailMessageBuilders;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.DataAccess.Entities;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryOptions;
using HomelessAnimals.Shared.Constants;
using HomelessAnimals.Shared.Enums;
using HomelessAnimals.Shared.Exceptions;
using Repository = HomelessAnimals.DataAccess.Entities;
using SignUpRequest = HomelessAnimals.BusinessLogic.Models.SignUpRequest;

namespace HomelessAnimals.BusinessLogic.Services
{
    public class SignUpRequestService : ISignUpRequestService
    {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IAccountService _accountService;
        private readonly IDataAccessFactory _dataFactory;
        private readonly IMapper _mapper;

        public SignUpRequestService(IDataAccessFactory dataAccessFactory, IPasswordHashingService passwordHashingService,
            IAccountService accountService, IMapper mapper)
        {
            _passwordHashingService = passwordHashingService;
            _accountService = accountService;
            _dataFactory = dataAccessFactory;
            _mapper = mapper;
        }

        public async Task<SignUpRequest> GetSignUpRequestInfo(int id)
        {
            var options = new SignUpRequestQueryOptions
            {
                IncludeCity = true,
                AsNoTracking = true,
            };

            var signUpRequest = await _dataFactory.SignUpRequestRepository.GetSignUpRequest(id, options)
                ?? throw new NotFoundException("Sign up request could not be found");

            return _mapper.Map<SignUpRequest>(signUpRequest);
        }

        public async Task SubmitSignUpRequest(Models.SignUpRequest model)
        {
            var emailExists = await _dataFactory.AccountRepository.AccountAlreadyExists(model.Email);

            if (emailExists)
                throw new AccountAlreadyExistsException("An account with this email already exists");

            var nicknameExists = await _dataFactory.VolunteerProfileRepository.VolunteerAlreadyExists(model.FirstName);

            if (nicknameExists)
                throw new AccountAlreadyExistsException("An account with this Playdek name already exists");

            var requestExists = await _dataFactory.SignUpRequestRepository.SignUpRequestAlreadyExists(model.Email);

            if (requestExists)
                throw new AccountAlreadyExistsException("A submit request with this email or Playdek already exists");

            model.SubmittedOn = DateTime.UtcNow;
            model.Status = SignUpRequestStatus.Pending;

            var signUpRequest = _mapper.Map<Repository.SignUpRequest>(model);

            _dataFactory.SignUpRequestRepository.AddSignUpRequest(signUpRequest);

            await _dataFactory.SaveChanges();
        }

        public async Task<List<SignUpRequestInfoShort>> GetSignUpRequests()
        {
            var requests = await _dataFactory.SignUpRequestRepository.GetSignUpRequests();

            return _mapper.Map<List<SignUpRequestInfoShort>>(requests);
        }

        public async Task ChangeSignUpRequestStatus(int id, SignUpRequestStatus status)
        {
            var action = status switch
            {
                SignUpRequestStatus.Rejected => RejectSignUpRequest(id),
                SignUpRequestStatus.Accepted => AcceptSignUpRequest(id),
                _ => throw new NotImplementedException()
            };

            await action;
        }

        private async Task RejectSignUpRequest(int id)
        {
            var options = new SignUpRequestQueryOptions();

            var signUpRequest = await _dataFactory.SignUpRequestRepository.GetSignUpRequest(id, options)
                ?? throw new NotFoundException("Sign up request could not be found");

            signUpRequest.Status = SignUpRequestStatus.Rejected;

            await _dataFactory.SaveChanges();
        }

        private async Task AcceptSignUpRequest(int id)
        {
            var options = new SignUpRequestQueryOptions { IncludeCity = true };

            var signUpRequest = await _dataFactory.SignUpRequestRepository.GetSignUpRequest(id, options)
                ?? throw new NotFoundException("Sign up request could not be found");

            var volunteer = _mapper.Map<Repository.Volunteer>(signUpRequest);

            var account = new Account
            {
                Email = signUpRequest.Email,
                Password = _passwordHashingService.Encrypt(Guid.NewGuid().ToString(), out var salt),
                IsEmailValid = true,
                HasAllowedEmailNotifications = true,
                Salt = salt
            };

            var userRole = await _dataFactory.AccountRepository.GetRole(RoleNames.User);

            account.RoleAssignments.Add(new RoleAssignment
            {
                Role = userRole,
                Scopes = [ new Repository.Scope
                    {
                        Level = ScopeLevel.Volounteer,
                        ResourceId = account.VolunteerId,
                    }
                ]
            });

            volunteer.Account = account;

            _dataFactory.VolunteerProfileRepository.CreateVoulonteer(volunteer);
            _dataFactory.SignUpRequestRepository.DeleteSignUpRequest(signUpRequest);

            await _dataFactory.SaveChanges();

            // send reset email
            await _accountService.ResetPassword(signUpRequest.Email, EmailTemplateNames.SetPassword);
        }
    }
}

