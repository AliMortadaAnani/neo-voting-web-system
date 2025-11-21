using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class NeoVoting_VoterHasVotedRequestDTOValidator : AbstractValidator<NeoVoting_VoterHasVotedRequestDTO>
    {
        public NeoVoting_VoterHasVotedRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.VotingToken).NotEmpty();
        }
    }
}