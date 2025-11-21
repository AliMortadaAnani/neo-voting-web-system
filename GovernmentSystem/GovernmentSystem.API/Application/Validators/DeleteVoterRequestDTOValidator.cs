using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class DeleteVoterRequestDTOValidator : AbstractValidator<DeleteVoterRequestDTO>
    {
        public DeleteVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
        }
    }
}