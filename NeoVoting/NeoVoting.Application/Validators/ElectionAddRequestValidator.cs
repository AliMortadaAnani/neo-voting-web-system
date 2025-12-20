using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class ElectionAddRequestValidator : AbstractValidator<ElectionAddRequest>
    {
        public ElectionAddRequestValidator()
        {
            // --- Schema-Compatible Rules (from ElectionConfiguration) ---

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("An election name is required.")
                .MaximumLength(100).WithMessage("The election name cannot exceed 100 characters.");

            RuleFor(x => x.NominationStartDate)
                .NotEmpty().WithMessage("A nomination start date is required.");

            RuleFor(x => x.NominationEndDate)
                .NotEmpty().WithMessage("A nomination end date is required.");

            RuleFor(x => x.VotingStartDate)
                .NotEmpty().WithMessage("A voting start date is required.");

            RuleFor(x => x.VotingEndDate)
                .NotEmpty().WithMessage("A voting end date is required.");

            // --- Business Logic & User Input Rules (from Domain & DB Check Constraints) ---

            // Rule: Nomination start date must be in the future.
            RuleFor(x => x.NominationStartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("The nomination start date must be in the future.");

            // Rule: Nomination end date must be after start date.
            RuleFor(x => x.NominationEndDate)
                .GreaterThan(x => x.NominationStartDate)
                .WithMessage("The nomination end date must be after the nomination start date.");

            // Rule: Voting start date must be after nomination has ended.
            RuleFor(x => x.VotingStartDate)
                .GreaterThanOrEqualTo(x => x.NominationEndDate) // Using >= as per your DB constraint
                .WithMessage("The voting period must begin on or after the nomination period ends.");

            // Rule: Voting end date must be after voting has started.
            RuleFor(x => x.VotingEndDate)
                .GreaterThan(x => x.VotingStartDate)
                .WithMessage("The voting end date must be after the voting start date.");
        }
    }
}