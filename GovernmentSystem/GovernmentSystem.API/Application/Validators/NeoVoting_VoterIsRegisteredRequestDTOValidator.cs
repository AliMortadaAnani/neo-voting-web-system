using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class NeoVoting_VoterIsRegisteredRequestDTOValidator : AbstractValidator<NeoVoting_VoterIsRegisteredRequestDTO>
    {
        public NeoVoting_VoterIsRegisteredRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
            RuleFor(x => x.VotingToken).NotNull().NotEmpty();
            RuleFor(x => x.RegisteredUsername).NotNull().NotEmpty().MaximumLength(100); ;
        }
    }
}