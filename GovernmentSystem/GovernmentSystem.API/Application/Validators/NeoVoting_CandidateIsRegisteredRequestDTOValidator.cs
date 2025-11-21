using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class NeoVoting_CandidateIsRegisteredRequestDTOValidator : AbstractValidator<NeoVoting_CandidateIsRegisteredRequestDTO>
    {
        public NeoVoting_CandidateIsRegisteredRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.NominationToken).NotEmpty();
        }
    }
}