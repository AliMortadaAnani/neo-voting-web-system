using FluentValidation;
using NeoVoting.Application.RequestDTOs.AuthDTOs;

namespace NeoVoting.Application.Validators.AuthDTOs
{
    public class Login_RequestDTOValidator : AbstractValidator<Login_RequestDTO>
    {
        public Login_RequestDTOValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}