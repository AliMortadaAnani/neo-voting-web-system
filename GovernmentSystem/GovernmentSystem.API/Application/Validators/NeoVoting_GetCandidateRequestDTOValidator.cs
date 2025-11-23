using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class NeoVoting_GetCandidateRequestDTOValidator : AbstractValidator<NeoVoting_GetCandidateRequestDTO>
    {
        public NeoVoting_GetCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
            RuleFor(x => x.NominationToken).NotNull().NotEmpty();
        }
    }
}