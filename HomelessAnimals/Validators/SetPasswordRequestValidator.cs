using FluentValidation;
using HomelessAnimals.Models;

namespace HomelessAnimals.Validators
{
    public class SetPasswordRequestValidator : AbstractValidator<SetPasswordRequest>
    {
        public SetPasswordRequestValidator()
        {
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.Token).NotEmpty();
        }
    }
}
