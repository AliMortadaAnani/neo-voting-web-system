using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class NeoVoting_GetVoterRequestDTOValidator : AbstractValidator<NeoVoting_GetVoterRequestDTO>
    {
        public NeoVoting_GetVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.VotingToken).NotEmpty();
        }
    }
}