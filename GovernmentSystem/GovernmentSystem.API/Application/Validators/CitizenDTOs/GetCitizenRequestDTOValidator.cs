using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;

namespace GovernmentSystem.API.Application.Validators.CitizenDTOs
{
    public class GetCitizenRequestDTOValidator : AbstractValidator<GetCitizenRequestDTO>
    {
        public GetCitizenRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
