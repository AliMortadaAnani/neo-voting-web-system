using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;

namespace GovernmentSystem.API.Application.Validators.VoterDTOs
{
    public class GetVoterRequestDTOValidator : AbstractValidator<GetVoterRequestDTO>
    {
        public GetVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
