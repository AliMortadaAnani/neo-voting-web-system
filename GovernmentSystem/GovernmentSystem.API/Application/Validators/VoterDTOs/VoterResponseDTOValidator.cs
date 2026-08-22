using FluentValidation;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;

namespace GovernmentSystem.API.Application.Validators.VoterDTOs
{
    public class VoterResponseDTOValidator : AbstractValidator<VoterResponseDTO>
    {
        public VoterResponseDTOValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NationalId).NotEmpty();
            RuleFor(x => x.CitizenId).NotEmpty();
            RuleFor(x => x.VotingToken).NotEmpty();
            RuleFor(x => x.HashedData).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.GovernorateId).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
        }
    }
}
