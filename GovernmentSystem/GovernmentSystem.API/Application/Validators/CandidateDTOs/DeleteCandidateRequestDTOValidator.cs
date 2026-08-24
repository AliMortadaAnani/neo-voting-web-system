using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;

namespace GovernmentSystem.API.Application.Validators.CandidateDTOs
{
    public class DeleteCandidateRequestDTOValidator : AbstractValidator<DeleteCandidateRequestDTO>
    {
        public DeleteCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}