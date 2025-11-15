using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class CandidateProfileUpdateRequestValidator : AbstractValidator<CandidateProfileUpdateRequest>
    {
        public CandidateProfileUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The profile ID is required for an update operation.");

            // The validation rules for the text content are the same for create and update.
            RuleFor(x => x.Goals)
                .NotEmpty().WithMessage("Please provide the candidate's goals.")
                .MaximumLength(1000).WithMessage("The goals description cannot exceed 1000 characters.")
                .MinimumLength(10).WithMessage("Please provide a more detailed description of your goals (at least 10 characters).");

            RuleFor(x => x.NominationReasons)
                .NotEmpty().WithMessage("Please provide the reasons for your nomination.")
                .MaximumLength(1000).WithMessage("The nomination reasons cannot exceed 1000 characters.")
                .MinimumLength(10).WithMessage("Please provide more detail on your nomination reasons (at least 10 characters).");
        }
    }
}
