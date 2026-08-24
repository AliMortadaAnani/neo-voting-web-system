using FluentValidation;
using GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs;

namespace GovernmentSystem.API.Application.Validators.CitizenDTOs
{
    public class CitizenResponseDTOValidator : AbstractValidator<CitizenResponseDTO>
    {
        public CitizenResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.GovernorateId).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
        }
    }
}