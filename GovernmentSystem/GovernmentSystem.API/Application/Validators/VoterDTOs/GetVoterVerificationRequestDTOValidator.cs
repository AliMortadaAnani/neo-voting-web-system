using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;

namespace GovernmentSystem.API.Application.Validators.VoterDTOs
{
    public class GetVoterVerificationRequestDTOValidator : AbstractValidator<GetVoterVerificationRequestDTO>
    {
        public GetVoterVerificationRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.VotingToken)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}