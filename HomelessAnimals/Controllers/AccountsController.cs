using AutoMapper;
using FluentValidation;
using HomelessAnimals.BusinessLogic.EmailMessageBuilders;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.Models;
using HomelessAnimals.Shared.Constants;
using HomelessAnimals.Shared.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Business = HomelessAnimals.BusinessLogic.Models;

namespace HomelessAnimals.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IValidator<LoginRequest> _loginRequestValidator;
        private readonly IValidator<SetPasswordRequest> _setPasswordRequestValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService,
            IValidator<LoginRequest> loginRequestValidator,
            IValidator<SetPasswordRequest> setPasswordRequestValidator,
            IMapper mapper,
            ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _loginRequestValidator = loginRequestValidator;
            _setPasswordRequestValidator = setPasswordRequestValidator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation("Get authentication data of currently logged in user")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(AuthenticationInfo))]
        public IActionResult GetAuthenticationInfo()
        {
            var response = new AuthenticationInfo();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claims = HttpContext.User.Claims;

                response.VolunteerId = int.Parse(claims.FirstOrDefault(c => c.Type == nameof(AuthenticationInfo.VolunteerId))?.Value ?? "0");
                response.Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                response.Permissions = claims
                    .Where(x => x.Type == nameof(Permissions))
                    .Select(c => c.Value)
                    .ToArray();

                response.Scopes = claims
                    .Where(x => x.Type == nameof(AuthenticationInfo.Scopes))
                    .Select(s => new Business.Scope
                    {
                        Level = Enum.Parse<ScopeLevel>(s.Value.Split("/")[0]),
                        ResourceId = int.Parse(s.Value.Split("/")[1])
                    })
                    .ToArray();
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        [SwaggerOperation("Log user into the system")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(AuthenticationInfo))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ErrorResponse))]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            await _loginRequestValidator.ValidateAndThrowAsync(request);

            _logger.LogInformation("Login request for {Email} received", request.Email);

            var account = await _accountService.Authenticate(request.Email.ToLower(), request.Password);

            _logger.LogInformation("Authentication for {Email} successfully completed", request.Email);

            var claims = new List<Claim>
            {
                new(nameof(AuthenticationInfo.VolunteerId), account.PlayerId.ToString()),
                new(ClaimTypes.Name, account.Name.ToString()),
                new("LastValidated", DateTime.UtcNow.ToString())
            };

            var permissionsClaims = account.Permissions.Select(p => new Claim(nameof(Permissions), p));
            claims.AddRange(permissionsClaims);

            var scopesClaims = account.Scopes.Select(s => new Claim(nameof(AuthenticationInfo.Scopes), s.ToString()));
            claims.AddRange(scopesClaims);

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = request.RememberMe
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(_mapper.Map<AuthenticationInfo>(account));
        }

        [HttpPost]
        [Route("logout")]
        [SwaggerOperation("Log user out of the system")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return NoContent();
        }

        [HttpPost]
        [Route("password/reset")]
        [SwaggerOperation("Send password reset request")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            _logger.LogInformation("Reset password request for {Email} received", request.Email);

            await _accountService.ResetPassword(request.Email.ToLower(), EmailTemplateNames.ResetPassword);

            return NoContent();
        }

        [HttpPost]
        [SwaggerOperation("Set new password")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ErrorResponse))]
        [Route("password/set")]
        public async Task<IActionResult> SetPassword(SetPasswordRequest request)
        {
            _logger.LogInformation("Set password request received");

            await _setPasswordRequestValidator.ValidateAndThrowAsync(request);

            await _accountService.SetPassword(request.Password, request.Token);

            return NoContent();
        }
    }
}
