using FluentValidation;
using HomelessAnimals.Models;

namespace HomelessAnimals.Validators
{
    public class CreateAnimalValidator : AbstractValidator<Animal>
    {
        public CreateAnimalValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.VolunteerProfileId).NotEmpty();
            RuleFor(x => x.AnimalAdmins).NotEmpty();
        }
    }
}
