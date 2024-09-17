using FluentValidation;
using HomelessAnimals.Models;

namespace HomelessAnimals.Validators
{
    public class SubmitSignUpRequestValidator : AbstractValidator<SubmitSignUpRequest>
    {
        public SubmitSignUpRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            // regex is for E.164 format
            RuleFor(x => x.PhoneNumber).Matches(@"^\+\d{1,15}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber));
            RuleFor(x => x.CityId).NotEmpty();
            RuleFor(x => x.TelegramName).NotEmpty().MaximumLength(50);
        }
    }
}
