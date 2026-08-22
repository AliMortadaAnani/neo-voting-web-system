using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;

namespace GovernmentSystem.API.Application.Validators.VoterDTOs
{
    public class CreateVoterRequestDTOValidator : AbstractValidator<CreateVoterRequestDTO>
    {
        public CreateVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
