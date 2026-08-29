using FluentValidation;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class UserOperations_ResponseDTOValidator : AbstractValidator<UserOperations_ResponseDTO>
    {
        public UserOperations_ResponseDTOValidator()
        {
            RuleFor(x => x.ApplicationUserId).NotNull();
            RuleFor(x => x.AccountId).NotNull();
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Governorate).NotNull();
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Role).NotNull();
            RuleFor(x => x.Message).NotEmpty();
            RuleFor(x => x.LockoutEnabled).NotNull();
            RuleFor(x => x.LockoutEnd).NotNull();
        }
    }
}
