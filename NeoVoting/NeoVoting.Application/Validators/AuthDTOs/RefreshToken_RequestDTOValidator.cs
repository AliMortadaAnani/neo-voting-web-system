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
            // Both are optional since they can come from cookies or body
            // No required rules needed
        }
    }
}
