using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class NeoVoting_VoterIsRegisteredRequestDTOValidator : AbstractValidator<NeoVoting_VoterIsRegisteredRequestDTO>
    {
        public NeoVoting_VoterIsRegisteredRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.VotingToken).NotEmpty();
        }
    }
}