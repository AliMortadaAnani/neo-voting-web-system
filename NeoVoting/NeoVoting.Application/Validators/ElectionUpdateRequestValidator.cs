using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class ElectionUpdateRequestValidator : AbstractValidator<ElectionUpdateRequest>
    {
        public ElectionUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The election ID is required for an update operation.");

            // The validation rules for the properties are identical for both Add and Update.
            // In a real project, you could create a reusable base validator or rule set
            // to avoid duplicating the rules below.

            // --- Schema-Compatible Rules ---
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

            // --- Business Logic & User Input Rules ---
            // NOTE: The rule that the start date must be in the future might be too restrictive
            // for an update. An admin might need to edit an election that has already been created.
            // For that reason, I am commenting it out here.
            /*
            RuleFor(x => x.NominationStartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("The nomination start date must be in the future.");
            */

            RuleFor(x => x.NominationEndDate)
                .GreaterThan(x => x.NominationStartDate)
                .WithMessage("The nomination end date must be after the nomination start date.");

            RuleFor(x => x.VotingStartDate)
                .GreaterThanOrEqualTo(x => x.NominationEndDate)
                .WithMessage("The voting period must begin on or after the nomination period ends.");

            RuleFor(x => x.VotingEndDate)
                .GreaterThan(x => x.VotingStartDate)
                .WithMessage("The voting end date must be after the voting start date.");
        }
    }
}