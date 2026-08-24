using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;

namespace GovernmentSystem.API.Application.Validators.CandidateDTOs
{
    public class GetCandidateVerificationRequestDTOValidator : AbstractValidator<GetCandidateVerificationRequestDTO>
    {
        public GetCandidateVerificationRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.NominationToken)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}