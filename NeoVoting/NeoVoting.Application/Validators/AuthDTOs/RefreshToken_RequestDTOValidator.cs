using FluentValidation;
using NeoVoting.Application.RequestDTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AuthDTOs
{
    public class RefreshToken_RequestDTOValidator : AbstractValidator<RefreshToken_RequestDTO>
    {
        public RefreshToken_RequestDTOValidator()
        {

            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access token is required.");

            //RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required.");
            // we will extract the refresh token from the cookie, so we don't need to validate it here
        }
    }
}
