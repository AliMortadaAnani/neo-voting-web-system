using FluentValidation;

namespace NeoVoting.Application.RequestDTOs.Validators
{
    public class ElectionWinnerAddRequestValidator : AbstractValidator<ElectionWinnerAddRequest>
    {
        public ElectionWinnerAddRequestValidator()
        {
            // --- Schema-Compatible Rules (from ElectionWinnerConfiguration) ---

            RuleFor(x => x.ElectionId)
                .NotEmpty().WithMessage("The ID of the election is required to declare a winner.");

            RuleFor(x => x.CandidateProfileId)
                .NotEmpty().WithMessage("The ID of the winning candidate's profile is required.");

            // --- Business Logic & User Input Rules ---

            // This rule mirrors the check constraint from your domain entity.
            // It ensures the vote count, if provided, is not a negative number.
            RuleFor(x => x.VoteCount)
                .GreaterThanOrEqualTo(0).WithMessage("The vote count cannot be a negative number.")
                .When(x => x.VoteCount.HasValue); // This rule only applies if VoteCount is not null.
        }
    }
}