using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class NeoVoting_GetVoterRequestDTOValidator : AbstractValidator<NeoVoting_GetVoterRequestDTO>
    {
        public NeoVoting_GetVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
            RuleFor(x => x.VotingToken).NotNull().NotEmpty();
        }
    }
}