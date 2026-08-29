using FluentValidation;
using NeoVoting.Application.RequestDTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class UserCheckOrBanAccountByHashedData_RequestDTOValidator : AbstractValidator<UserCheckOrBanAccountByHashedData_RequestDTO>
    {
        public UserCheckOrBanAccountByHashedData_RequestDTOValidator()
        {
            RuleFor(x => x.HashedData).NotEmpty();
        }
    }
}
