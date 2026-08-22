using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.AdminDTOs;

namespace GovernmentSystem.API.Application.Validators.AdminDTOs
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
