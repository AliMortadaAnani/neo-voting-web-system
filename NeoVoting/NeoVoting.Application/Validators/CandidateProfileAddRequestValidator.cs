using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class CandidateProfileAddRequestValidator : AbstractValidator<CandidateProfileAddRequest>
    {
        public CandidateProfileAddRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("A user ID is required to create a candidate profile.");

            RuleFor(x => x.ElectionId)
                .NotEmpty().WithMessage("An election ID is required to create a candidate profile.");

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
