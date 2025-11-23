using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class DeleteVoterRequestDTOValidator : AbstractValidator<DeleteVoterRequestDTO>
    {
        public DeleteVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
        }
    }
}