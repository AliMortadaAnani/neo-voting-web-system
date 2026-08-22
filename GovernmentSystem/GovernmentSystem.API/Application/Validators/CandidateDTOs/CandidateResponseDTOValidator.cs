using FluentValidation;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;

namespace GovernmentSystem.API.Application.Validators.CandidateDTOs
{
    public class CandidateResponseDTOValidator : AbstractValidator<CandidateResponseDTO>
    {
        public CandidateResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.CitizenId).NotEmpty();
            RuleFor(x => x.NominationToken).NotEmpty();
            RuleFor(x => x.HashedData).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.GovernorateId).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
        }
    }
}
