using FluentValidation;
using GovernmentSystem.API.Application.ResponseDTOs.AdminDTOs;

namespace GovernmentSystem.API.Application.Validators.AdminDTOs
{
    public class AuthResponseValidator : AbstractValidator<AuthResponse>
    {
        public AuthResponseValidator()
        {
            RuleFor(x => x.IsSuccess).NotNull();
            RuleFor(x => x.Message).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Role).NotEmpty();
        }
    }
}