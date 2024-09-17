using FluentValidation;
using HomelessAnimals.Models;

namespace HomelessAnimals.Validators
{
    public class VolunteerEditValidator : AbstractValidator<AdminVolunteerEdit>
    {
        public VolunteerEditValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+\d{1,15}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.CityId).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.FullName).NotEmpty();
        }
    }
}
