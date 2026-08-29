using FluentValidation;
using NeoVoting.Application.RequestDTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.AuthDTOs
{
    public class RegisterVoterOrCandidate_RequestDTOValidator : AbstractValidator<RegisterVoterOrCandidate_RequestDTO>
    {
        public RegisterVoterOrCandidate_RequestDTOValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).NotEmpty();
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.VotingOrNominationToken).NotEmpty();

            RuleFor(x => x)
                .Must(x => x.NewPassword == x.ConfirmPassword)
                .WithMessage("NewPassword and ConfirmPassword must match")
                .When(x => !string.IsNullOrEmpty(x.NewPassword) && !string.IsNullOrEmpty(x.ConfirmPassword));
        }
    }
}
