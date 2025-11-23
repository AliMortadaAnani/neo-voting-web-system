using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class GenerateNewTokenCandidateRequestDTOValidator : AbstractValidator<GenerateNewTokenCandidateRequestDTO>
    {
        public GenerateNewTokenCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
        }
    }
}