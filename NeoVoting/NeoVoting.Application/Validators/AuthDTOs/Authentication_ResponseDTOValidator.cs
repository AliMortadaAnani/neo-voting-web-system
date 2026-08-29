using FluentValidation;
using NeoVoting.Application.ResponseDTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AuthDTOs
{
    public class Authentication_ResponseDTOValidator : AbstractValidator<Authentication_ResponseDTO>
    {
        public Authentication_ResponseDTOValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
            RuleFor(x => x.AccessTokenExpiration).NotNull();
            RuleFor(x => x.RefreshTokenExpiration).NotNull();
            RuleFor(x => x.ApplicationUserId).NotNull();
            RuleFor(x => x.AccountId).NotNull();
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Governorate).NotNull();
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Role).NotNull();
        }
    }
}
