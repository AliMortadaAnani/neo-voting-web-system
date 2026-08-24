using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;

namespace GovernmentSystem.API.Application.Validators.VoterDTOs
{
    public class UpdateVoterRequestDTOValidator : AbstractValidator<UpdateVoterRequestDTO>
    {
        public UpdateVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}