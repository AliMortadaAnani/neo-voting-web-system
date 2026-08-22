using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;

namespace GovernmentSystem.API.Application.Validators.CandidateDTOs
{
    public class UpdateCandidateRequestDTOValidator : AbstractValidator<UpdateCandidateRequestDTO>
    {
        public UpdateCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
