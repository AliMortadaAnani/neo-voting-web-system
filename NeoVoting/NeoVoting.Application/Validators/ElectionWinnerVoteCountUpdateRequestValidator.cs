using FluentValidation;

namespace NeoVoting.Application.RequestDTOs.Validators
{
    public class ElectionWinnerVoteCountUpdateRequestValidator : AbstractValidator<ElectionWinnerVoteCountUpdateRequest>
    {
        public ElectionWinnerVoteCountUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The ID of the winner record is required for an update.");

            // The 'VoteCount' for an update should be a required, non-negative number.
            RuleFor(x => x.VoteCount)
                .NotNull().WithMessage("A valid vote count must be provided.")
                .GreaterThanOrEqualTo(0).WithMessage("The vote count cannot be a negative number.");
        }
    }
}
