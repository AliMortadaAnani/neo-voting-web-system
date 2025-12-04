using FluentValidation;
using GovernmentSystem.API.Application.AdminDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}