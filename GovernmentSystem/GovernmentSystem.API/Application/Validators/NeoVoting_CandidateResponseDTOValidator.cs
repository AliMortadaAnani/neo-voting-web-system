namespace GovernmentSystem.API.Application.Validators
{
    using FluentValidation;
    using GovernmentSystem.API.Application.ResponseDTOs;

    public class NeoVoting_CandidateResponseDTOValidator : AbstractValidator<NeoVoting_CandidateResponseDTO>
    {
        public NeoVoting_CandidateResponseDTOValidator()
        {
            RuleFor(x => x.GovernorateId)
                .NotNull();

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
