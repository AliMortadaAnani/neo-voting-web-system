using FluentValidation;
using NeoVoting.Application.RequestDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class UserResetPasswordByUserName_RequestDTOValidator : AbstractValidator<UserResetPasswordByUserName_RequestDTO>
    {
        public UserResetPasswordByUserName_RequestDTOValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(3);
            RuleFor(x => x.ConfirmPassword).NotEmpty();

            RuleFor(x => x)
                .Must(x => x.NewPassword == x.ConfirmPassword)
                .WithMessage("NewPassword and ConfirmPassword must match")
                .When(x => !string.IsNullOrEmpty(x.NewPassword) && !string.IsNullOrEmpty(x.ConfirmPassword));
        }
    }
}
