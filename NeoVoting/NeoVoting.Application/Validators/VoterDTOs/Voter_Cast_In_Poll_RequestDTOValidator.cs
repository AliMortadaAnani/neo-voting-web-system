using FluentValidation;
using NeoVoting.Application.RequestDTOs.VoterDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class Voter_Cast_In_Poll_RequestDTOValidator : AbstractValidator<Voter_Cast_In_Poll_RequestDTO>
    {
        public Voter_Cast_In_Poll_RequestDTOValidator()
        {
            RuleFor(x => x)
                .Must(x => x.SelectedPollAnswerId.HasValue && !string.IsNullOrEmpty(x.SelectedPollAnswer))
                .WithMessage("Either SelectedPollAnswerId and SelectedPollAnswer must be provided");
        }
    }
}
