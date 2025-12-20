using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class NeoVoting_CandidateIsRegisteredRequestDTOValidator : AbstractValidator<NeoVoting_CandidateIsRegisteredRequestDTO>
    {
        public NeoVoting_CandidateIsRegisteredRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
            RuleFor(x => x.NominationToken).NotNull().NotEmpty();
            RuleFor(x => x.RegisteredUsername).NotNull().NotEmpty().MaximumLength(100); ;
        }
    }
}