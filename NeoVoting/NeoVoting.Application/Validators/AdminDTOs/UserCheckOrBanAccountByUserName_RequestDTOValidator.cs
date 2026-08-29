using FluentValidation;
using NeoVoting.Application.RequestDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class UserCheckOrBanAccountByUserName_RequestDTOValidator : AbstractValidator<UserCheckOrBanAccountByUserName_RequestDTO>
    {
        public UserCheckOrBanAccountByUserName_RequestDTOValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
        }
    }
}
