using AutoMapper;
using FluentValidation;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.Models;
using HomelessAnimals.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Business = HomelessAnimals.BusinessLogic.Models;

namespace HomelessAnimals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpRequestsController : Controller
    {
        private readonly ISignUpRequestService _signUpRequestService;
        private readonly IValidator<SubmitSignUpRequest> _submitSignUpRequestValidator;
        private readonly IValidator<UpdateSignUpRequestStatus> _updateSignUpRequestStatusValidator;
        private readonly ICaptchaValidationService _captchaValidationService;
        private readonly IMapper _mapper;
        private readonly ILogger<SignUpRequestsController> _logger;

        public SignUpRequestsController(ISignUpRequestService signUpRequestService,
            IValidator<SubmitSignUpRequest> submitSignUpRequestValidator,
            IValidator<UpdateSignUpRequestStatus> updateSignUpRequestStatusValidator,
            ICaptchaValidationService captchaValidationService,
            IMapper mapper,
            ILogger<SignUpRequestsController> logger)
        {
            _signUpRequestService = signUpRequestService;
            _submitSignUpRequestValidator = submitSignUpRequestValidator;
            _updateSignUpRequestStatusValidator = updateSignUpRequestStatusValidator;
            _captchaValidationService = captchaValidationService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [SwaggerOperation("Submit sign up request")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ErrorResponse))]
        public async Task<IActionResult> SubmitSignUpRequest(SubmitSignUpRequest request)
        {
            var isHuman = await _captchaValidationService.IsHuman(request.ReCaptchaToken);

            if (!isHuman)
                throw new ValidationException("We've identified unusual activity that suggests you may not be a human user");

            _mapper.Map(request, request);

            await _submitSignUpRequestValidator.ValidateAndThrowAsync(request);

            await _signUpRequestService.SubmitSignUpRequest(_mapper.Map<Business.SignUpRequest>(request));

            return NoContent();
        }

        [HttpGet]
        [Authorize(Policy = Permissions.SignUp.View)]
        [Route("{id}")]
        [SwaggerOperation("Get sign up request info")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(SignUpRequest))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSignUpRequestInfo(int id)
        {
            var request = await _signUpRequestService.GetSignUpRequestInfo(id);

            return Ok(_mapper.Map<SignUpRequest>(request));
        }

        [HttpPatch]
        [Authorize(Policy = Permissions.SignUp.ChangeStatus)]
        [Route("{id}")]
        [SwaggerOperation("Change sign up request status")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeSignUpRequestStatus(int id, UpdateSignUpRequestStatus model)
        {
            await _updateSignUpRequestStatusValidator.ValidateAndThrowAsync(model);

            await _signUpRequestService.ChangeSignUpRequestStatus(id, model.Status);

            return NoContent();
        }

        [HttpGet]
        [Authorize(Policy = Permissions.SignUp.View)]
        [SwaggerOperation("Get sign up requests list")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<SignUpRequestInfoShort>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetSignUpRequests()
        {
            var requests = await _signUpRequestService.GetSignUpRequests();

            return Ok(_mapper.Map<List<SignUpRequestInfoShort>>(requests));
        }
    }
}
