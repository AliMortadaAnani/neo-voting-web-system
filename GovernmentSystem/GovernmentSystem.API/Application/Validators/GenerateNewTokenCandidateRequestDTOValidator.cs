using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class GenerateNewTokenCandidateRequestDTOValidator : AbstractValidator<GenerateNewTokenCandidateRequestDTO>
    {
        public GenerateNewTokenCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
        }
    }
}