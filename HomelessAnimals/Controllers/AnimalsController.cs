using AutoMapper;
using FluentValidation;
using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.Models;
using HomelessAnimals.QueryParams;
using HomelessAnimals.Shared.Constants;
using HomelessAnimals.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Dynamic.Core;
using Business = HomelessAnimals.BusinessLogic.Models;
using BusinessQueryParams = HomelessAnimals.BusinessLogic.QueryParams;

namespace HomelessAnimals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAnimalService _animalService;
        private readonly IValidator<Animal> _animalValidator;
        private readonly IScopeVerificationService _scopeVerificationService;
        public AnimalsController(
            IAnimalService animalService,
            IValidator<Animal> animalValidator,
            IScopeVerificationService scopeVerificationService,
            IMapper mapper
            )
        {
            _animalService = animalService;
            _animalValidator = animalValidator;
            _scopeVerificationService = scopeVerificationService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Animal.Create)]
        [SwaggerOperation("Create an animal")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAnimalProfile(Animal model)
        {
            _mapper.Map(model, model);

            await _animalValidator.ValidateAndThrowAsync(model);

            await _animalService.CreateAnimal(_mapper.Map<Business.Animal>(model));

            return NoContent();
        }

        [HttpGet]
        [SwaggerOperation("Get list of animals")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<Animal>))]
        public async Task<IActionResult> GetAnimals([FromQuery] GetAnimalsQueryParams queryParams)
        {
            var parameters = _mapper.Map<BusinessQueryParams.GetAnimalsQueryParams>(queryParams);

            var animals = await _animalService.GetAnimals(parameters);

            return Ok(_mapper.Map<PagedResult<Animal>>(animals));
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Get animal")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Animal))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnimal(int id)
        {
            var tournament = await _animalService.GetAnimal(id);

            return Ok(_mapper.Map<Animal>(tournament));
        }

        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation("Edit animal")]
        [Authorize(Policy = Permissions.Animal.Edit)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditAnimal(int id, Animal model)
        {
            if (!_scopeVerificationService.ValidateScopeForAnimalManage(HttpContext.User.Claims, id))
                throw new NotPermittedException();

            await _animalValidator.ValidateAndThrowAsync(model);

            var tournament = _mapper.Map<Business.Animal>(model);

            await _animalService.EditAnimal(id, tournament);

            if (_scopeVerificationService.ValidateScopeForAnimalAdminReassignment(HttpContext.User.Claims))
                await _animalService.EditAnimalAdmins(id, model.AnimalAdmins);

            return NoContent();
        }
    }
}
