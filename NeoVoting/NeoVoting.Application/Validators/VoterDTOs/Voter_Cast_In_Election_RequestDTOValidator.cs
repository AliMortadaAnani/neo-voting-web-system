using FluentValidation;
using NeoVoting.Application.RequestDTOs.VoterDTOs;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class Voter_Cast_In_Election_RequestDTOValidator : AbstractValidator<Voter_Cast_In_Election_RequestDTO>
    {
        public Voter_Cast_In_Election_RequestDTOValidator()
        {
            RuleFor(x => x.SelectedCandidateProfileId).NotNull().WithMessage("SelectedCandidateProfileId is required.");
        }
    }
}