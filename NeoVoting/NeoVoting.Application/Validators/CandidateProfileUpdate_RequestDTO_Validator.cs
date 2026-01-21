using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class CandidateProfileUpdate_RequestDTO_Validator : AbstractValidator<CandidateProfileUpdate_RequestDTO>
    {
        public CandidateProfileUpdate_RequestDTO_Validator()
        {
            RuleFor(x => x.Goals)
                .NotEmpty().WithMessage("Candidate goals are required.")
                .MaximumLength(1000).WithMessage("Goals cannot exceed 1000 characters.");

            RuleFor(x => x.NominationReasons)
                .NotEmpty().WithMessage("Nomination reasons are required.")
                .MaximumLength(1000).WithMessage("Nomination reasons cannot exceed 1000 characters.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .NotEqual(Guid.Empty).WithMessage("National ID cannot be empty.");

            RuleFor(x => x.NominationToken)
                .NotEmpty().WithMessage("Nomination token is required.")
                .NotEqual(Guid.Empty).WithMessage("Nomination token cannot be empty.");
        }
    }
}
