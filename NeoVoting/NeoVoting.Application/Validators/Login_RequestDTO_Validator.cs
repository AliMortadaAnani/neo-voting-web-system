using FluentValidation;
using NeoVoting.Application.AuthDTOs;

namespace NeoVoting.Application.Validators
{
    public class Login_RequestDTO_Validator : AbstractValidator<Login_RequestDTO>
    {
        public Login_RequestDTO_Validator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
