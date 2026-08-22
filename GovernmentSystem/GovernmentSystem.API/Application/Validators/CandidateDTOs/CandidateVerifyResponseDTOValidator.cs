using FluentValidation;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;

namespace GovernmentSystem.API.Application.Validators.CandidateDTOs
{
    public class CandidateVerifyResponseDTOValidator : AbstractValidator<CandidateVerifyResponseDTO>
    {
        public CandidateVerifyResponseDTOValidator()
        {
            RuleFor(x => x.HashedData).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.GovernorateId).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
        }
    }
}
