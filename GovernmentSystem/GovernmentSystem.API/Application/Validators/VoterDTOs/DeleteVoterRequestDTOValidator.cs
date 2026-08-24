using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;

namespace GovernmentSystem.API.Application.Validators.VoterDTOs
{
    public class DeleteVoterRequestDTOValidator : AbstractValidator<DeleteVoterRequestDTO>
    {
        public DeleteVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}