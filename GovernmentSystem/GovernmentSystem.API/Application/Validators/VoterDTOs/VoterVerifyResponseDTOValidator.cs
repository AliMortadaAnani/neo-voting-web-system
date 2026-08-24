using FluentValidation;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;

namespace GovernmentSystem.API.Application.Validators.VoterDTOs
{
    public class VoterVerifyResponseDTOValidator : AbstractValidator<VoterVerifyResponseDTO>
    {
        public VoterVerifyResponseDTOValidator()
        {
            RuleFor(x => x.HashedData).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.Governorate).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
        }
    }
}