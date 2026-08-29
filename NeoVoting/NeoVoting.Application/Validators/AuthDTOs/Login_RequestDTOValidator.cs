using FluentValidation;
using NeoVoting.Application.RequestDTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
