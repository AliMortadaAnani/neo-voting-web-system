using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class DeleteCandidateRequestDTOValidator : AbstractValidator<DeleteCandidateRequestDTO>
    {
        public DeleteCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
        }
    }
}