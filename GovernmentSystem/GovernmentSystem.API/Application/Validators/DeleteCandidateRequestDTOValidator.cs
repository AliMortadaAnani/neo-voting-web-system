using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class DeleteCandidateRequestDTOValidator : AbstractValidator<DeleteCandidateRequestDTO>
    {
        public DeleteCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
        }
    }
}