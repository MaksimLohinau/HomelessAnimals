using FluentValidation;
using HomelessAnimals.Models;

namespace HomelessAnimals.Validators
{
    public class UpdateSignUpRequestStatusValidator : AbstractValidator<UpdateSignUpRequestStatus>
    {
        public UpdateSignUpRequestStatusValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
