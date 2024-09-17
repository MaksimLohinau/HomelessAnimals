using AutoMapper;
using HomelessAnimals.BusinessLogic.Interfaces;
using Business = HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.Extensions;
using HomelessAnimals.Models;
using HomelessAnimals.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FluentValidation;
using HomelessAnimals.Shared.Constants;
using System.Linq.Dynamic.Core;
using BusinessQueryParams = HomelessAnimals.BusinessLogic.QueryParams;
using HomelessAnimals.QueryParams;
using HomelessAnimals.BusinessLogic.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace HomelessAnimals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteersController : Controller
    {
        private readonly IVolunteerService _volunteerService;
        private readonly IMapper _mapper;
        private readonly IValidator<AdminVolunteerEdit> _volunteerEditValidator;
        private readonly ILogger<VolunteersController> _logger;
        private readonly IValidator<Volunteer> _volunteerValidator;

        public VolunteersController(IVolunteerService volunteerService,
            IMapper mappper,
            ILogger<VolunteersController> logger,
            IValidator<AdminVolunteerEdit> volunteerEditValidator,
            IValidator<Volunteer> volunteerValidator)
        {
            _volunteerService = volunteerService;
            _mapper = mappper;
            _logger = logger;
            _volunteerEditValidator = volunteerEditValidator;
            _volunteerValidator = volunteerValidator;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Get volunteer")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Volunteer))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVolunteer(int id)
        {
            var tournament = await _volunteerService.GetVolunteerProfile(id);

            return Ok(_mapper.Map<Volunteer>(tournament));
        }

        [HttpPatch]
        [Route("edit")]
        [Authorize]
        [SwaggerOperation("Update voluteer info")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditPlayer(Volunteer request)
        {
            if (request.Id != HttpContext.GetUserId())
                throw new NotPermittedException();

            _mapper.Map(request, request);

            await _volunteerValidator.ValidateAndThrowAsync(request);

            await _volunteerService.EditVolunteerProfile(_mapper.Map<Business.Volunteer>(request));

            return NoContent();
        }

        [HttpPatch]
        [Route("edit/admin")]
        [Authorize(Policy = Permissions.Volunteer.Edit)]
        [SwaggerOperation("Update volunteer info by admin")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditPlayer(AdminVolunteerEdit request)
        {
            _mapper.Map(request, request);

            await _volunteerEditValidator.ValidateAndThrowAsync(request);

            await _volunteerService.EditVolunteer(_mapper.Map<Business.Volunteer>(request));

            _logger.LogInformation("Admin #{Id} edited account #{AccountId}",
                HttpContext.User.Claims.First(x => x.Type == nameof(AuthenticationInfo.VolunteerId)),
                request.Id);

            return NoContent();
        }

        [HttpGet]
        [SwaggerOperation("Get list of volunteers with animals")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PagedResult<Volunteer>))]
        public async Task<ActionResult<PagedResult<Volunteer>>> GetVolunteers([FromQuery] GetVolunteersQueryParams queryParams)
        {
            var parameters = _mapper.Map<BusinessQueryParams.GetVolunteersQueryParams>(queryParams);

            var players = await _volunteerService.GetVolunteerProfiles(parameters);

            return Ok(_mapper.Map<PagedResult<Volunteer>>(players));
        }

        [HttpGet]
        [Route("{id}/profile/email")]
        [EnableRateLimiting("HiddenSensitiveValuesPolicy")]
        [Authorize(Policy = Permissions.Volunteer.ViewEmail)]
        [SwaggerOperation("Get volunteer email address")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(VolunteerEmailAddress))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetPlayerEmailAddress(int id)
        {
            var email = await _volunteerService.GetVolunteerEmail(id);

            return Ok(new VolunteerEmailAddress
            {
                EmailAddress = email
            });
        }

        [HttpGet]
        [Route("{id}/profile/phone")]
        [EnableRateLimiting("HiddenSensitiveValuesPolicy")]
        [Authorize(Policy = Permissions.Volunteer.ViewTelegram)]
        [SwaggerOperation("Get volunteer phone number")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(VolunteerTelegrammAccount))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetPlayerPhoneNumber(int id)
        {
            var phoneNumber = await _volunteerService.GetVolunteerPhoneNumber(id);

            return Ok(new VolunteerTelegrammAccount
            {
                TelegrammAccount = phoneNumber
            });
        }
    }
}
