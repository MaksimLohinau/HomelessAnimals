using AutoMapper;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.DataAccess.QueryOptions;
using HomelessAnimals.Shared.Exceptions;
using HomelessAnimals.Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomelessAnimals.BusinessLogic.Services
{
    public class AccountService : IAccountService
    {
        private readonly IDataAccessFactory _dataFactory;
        private readonly IMapper _mapper;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateBuilder _emailTemplateBuilder;
        private readonly ILogger<AccountService> _logger;
        private readonly SetPasswordSettings _options;

        public AccountService(
            IDataAccessFactory dataFactory,
            IPasswordHashingService passwordHashingService,
            IEmailSender emailSender,
            IEmailTemplateBuilder emailTemplateBuilder,
            ILogger<AccountService> logger,
            IOptions<SetPasswordSettings> options,
            IMapper mapper)
        {
            _dataFactory = dataFactory;
            _mapper = mapper;
            _passwordHashingService = passwordHashingService;
            _logger = logger;
            _options = options.Value;
            _passwordHashingService = passwordHashingService;
            _emailSender = emailSender;
            _emailTemplateBuilder = emailTemplateBuilder;
        }

        public async Task<AuthenticationInfo> Authenticate(string email, string password)
        {
            var options = new AccountQueryOptions
            {
                AsNoTracking = true,
                IncludeVolunteerProfile = true,
                IncludeRoleAssignments = true,
                IncludeRoles = true,
                IncludePermissions = true,
                IncludeScopes = true
            };

            var account = await _dataFactory.AccountRepository.GetAccount(email, options);

            if (account is null || !_passwordHashingService
                .VerifyPassword(password, account.Password, account.Salt))
            {
                _logger.LogInformation("Authentication for {Email} failed", email);
                throw new FailedAuthenticationException();
            }

            return _mapper.Map<AuthenticationInfo>(account);
        }

        public async Task<PermissionsInfo> GetPermissions(int id)
        {
            var options = new AccountQueryOptions
            {
                AsNoTracking = true,
                IncludeRoleAssignments = true,
                IncludeRoles = true,
                IncludePermissions = true,
                IncludeScopes = true
            };

            var account = await _dataFactory.AccountRepository.GetAccount(id, options);

            return _mapper.Map<PermissionsInfo>(account);
        }

        public async Task ResetPassword(string email, string templateName)
        {
            var options = new AccountQueryOptions { AsNoTracking = true, IncludeVolunteerProfile = true };
            var account = await _dataFactory.AccountRepository.GetAccount(email, options);

            if (account is null)
            {
                _logger.LogInformation("Reset password for {Email} failed - no account found", email);
                return;
            }

            var resetToken = _passwordHashingService
                .GeneratePasswordResetToken(account.VolunteerId, out var token);

            _dataFactory.ResetPasswordTokenRepository.AddToken(resetToken);
            await _dataFactory.SaveChanges();

            _logger.LogInformation("Reset password for {Email} - password reset token created", email);

            var url = $"{_options.SetPasswordUrl}?token={token}";

            var message = await _emailTemplateBuilder
                .CreateHtmlTemplate(templateName, account.Volunteer.LastName, url);

            _logger.LogInformation("Reset password for {Email} - email message created", email);

            await _emailSender.Send(account.Email, "Password reset", message);

            _logger.LogInformation("Reset password for {Email} - email successfully sent", email);
        }

        public async Task SetPassword(string password, string token)
        {
            var hashedToken = _passwordHashingService.GetHashedPasswordResetToken(token);
            var resetToken = await _dataFactory.ResetPasswordTokenRepository.GetToken(hashedToken);

            if (resetToken is null)
            {
                _logger.LogInformation("Set password request failed due to expired or invalid token {Token}", token);
                throw new ResetPasswordFailedException();
            }

            var options = new AccountQueryOptions { };
            var account = await _dataFactory.AccountRepository.GetAccount(resetToken.AccountId, options);

            var newHashedPassword = _passwordHashingService.Encrypt(password, out var salt);

            account.Salt = salt;
            account.Password = newHashedPassword;

            await _dataFactory.ResetPasswordTokenRepository.RemoveTokens(account.VolunteerId);
            await _dataFactory.SaveChanges();

            _logger.LogInformation("Set password request for user #{Id} successfully completed", account.VolunteerId);
        }
    }
}
