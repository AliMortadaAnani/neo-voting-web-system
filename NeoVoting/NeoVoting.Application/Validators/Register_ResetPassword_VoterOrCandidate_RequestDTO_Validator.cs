using FluentValidation;
using NeoVoting.Application.AuthDTOs;

namespace NeoVoting.Application.Validators
{
    public class Register_ResetPassword_VoterOrCandidate_RequestDTO_Validator : AbstractValidator<Register_ResetPassword_VoterOrCandidate_RequestDTO>
    {
        public Register_ResetPassword_VoterOrCandidate_RequestDTO_Validator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z0-9_.-]+$").WithMessage("Username can only contain letters, numbers, underscores, dots, and hyphens.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(3).WithMessage("Password must be at least 3 characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .NotEqual(Guid.Empty).WithMessage("National ID cannot be empty.");

            RuleFor(x => x.VotingOrNominationToken)
                .NotEmpty().WithMessage("Voting or Nomination token is required.")
                .NotEqual(Guid.Empty).WithMessage("Token cannot be empty.");
        }
    }
}
