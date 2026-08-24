using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;

namespace GovernmentSystem.API.Application.Validators.CitizenDTOs
{
    public class DeleteCitizenRequestDTOValidator : AbstractValidator<DeleteCitizenRequestDTO>
    {
        public DeleteCitizenRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}