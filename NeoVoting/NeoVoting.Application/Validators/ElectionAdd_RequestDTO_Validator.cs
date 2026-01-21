using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class ElectionAdd_RequestDTO_Validator : AbstractValidator<ElectionAdd_RequestDTO>
    {
        public ElectionAdd_RequestDTO_Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Election name is required.")
                .MaximumLength(100).WithMessage("Election name cannot exceed 100 characters.");

            RuleFor(x => x.NominationStartDate)
                .NotEmpty().WithMessage("Nomination start date is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("Nomination start date must be in the future.");

            RuleFor(x => x.NominationEndDate)
                .NotEmpty().WithMessage("Nomination end date is required.")
                .GreaterThan(x => x.NominationStartDate).WithMessage("Nomination end date must be after the nomination start date.");

            RuleFor(x => x.VotingStartDate)
                .NotEmpty().WithMessage("Voting start date is required.")
                .GreaterThanOrEqualTo(x => x.NominationEndDate).WithMessage("Voting start date must be after or equal to the nomination end date.");

            RuleFor(x => x.VotingEndDate)
                .NotEmpty().WithMessage("Voting end date is required.")
                .GreaterThan(x => x.VotingStartDate).WithMessage("Voting end date must be after the voting start date.");
        }
    }
}
