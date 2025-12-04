namespace GovernmentSystem.API.Application.Validators
{
    using FluentValidation;
    using GovernmentSystem.API.Application.ResponseDTOs;

    public class VoterResponseDTOValidator : AbstractValidator<VoterResponseDTO>
    {
        public VoterResponseDTOValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.NationalId)
                .NotEmpty();

            RuleFor(x => x.VotingToken)
                .NotEmpty();

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

            RuleFor(x => x.Voted)
                .NotNull();
        }
    }
}