using FluentValidation;
using NeoVoting.Application.RequestDTOs.VoterDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class Voter_Cast_In_Election_RequestDTOValidator : AbstractValidator<Voter_Cast_In_Election_RequestDTO>
    {
        public Voter_Cast_In_Election_RequestDTOValidator()
        {
            RuleFor(x => x.SelectedCandidateProfileIds).NotEmpty();
            RuleFor(x => x.SelectedCandidateProfileIds)
                .Must(x => x.Count == 5)
                .WithMessage("You must select exactly 5 candidates")
                .When(x => x.SelectedCandidateProfileIds != null);
        }
    }
}
