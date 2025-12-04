using FluentValidation;
using GovernmentSystem.API.Application.ResponseDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class CandidateResponseDTOValidator : AbstractValidator<CandidateResponseDTO>
    {
        public CandidateResponseDTOValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.NationalId)
                .NotEmpty();

            RuleFor(x => x.NominationToken)
                .NotEmpty();

            RuleFor(x => x.GovernorateId)
                .NotNull(); // adjust if GovernorateId is a struct/value object with its own validation

            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.DateOfBirth)
                .NotEmpty();

            RuleFor(x => x.Gender)
                .NotEmpty();

            RuleFor(x => x.EligibleForElection)
                .NotNull();

            RuleFor(x => x.ValidToken)
                .NotNull();

            RuleFor(x => x.IsRegistered)
                .NotNull();
        }
    }
}