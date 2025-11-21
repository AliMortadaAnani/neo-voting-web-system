using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class NeoVoting_GetCandidateRequestDTOValidator : AbstractValidator<NeoVoting_GetCandidateRequestDTO>
    {
        public NeoVoting_GetCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.NominationToken).NotEmpty();
        }
    }
}