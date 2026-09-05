using FluentValidation;
using NeoVoting.Application.RequestDTOs.VoterDTOs;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class Voter_Cast_In_Poll_RequestDTOValidator : AbstractValidator<Voter_Cast_In_Poll_RequestDTO>
    {
        public Voter_Cast_In_Poll_RequestDTOValidator()
        {
            RuleFor(x => x.SelectedPollAnswerId).NotNull().WithMessage("SelectedPollAnswerId is required.");
        }
    }
}