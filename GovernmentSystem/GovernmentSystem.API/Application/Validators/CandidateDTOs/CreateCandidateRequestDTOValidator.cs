using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;

namespace GovernmentSystem.API.Application.Validators.CandidateDTOs
{
    public class CreateCandidateRequestDTOValidator : AbstractValidator<CreateCandidateRequestDTO>
    {
        public CreateCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}