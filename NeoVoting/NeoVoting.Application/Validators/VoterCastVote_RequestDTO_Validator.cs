using FluentValidation;
using NeoVoting.Application.RequestDTOs;

namespace NeoVoting.Application.Validators
{
    public class VoterCastVote_RequestDTO_Validator : AbstractValidator<VoterCastVote_RequestDTO>
    {
        // Exact number of candidates a voter must select (5 per governorate)
        private const int RequiredCandidateSelections = 5;

        public VoterCastVote_RequestDTO_Validator()
        {
            RuleFor(x => x.SelectedCandidateProfileIds)
                .NotNull().WithMessage("Selected candidates list is required.")
                .NotEmpty().WithMessage("Candidate selections are required.")
                .Must(ids => ids != null && ids.Count == RequiredCandidateSelections)
                    .WithMessage($"Exactly {RequiredCandidateSelections} candidates must be selected.")
                .Must(ids => ids != null && ids.All(id => id != Guid.Empty))
                    .WithMessage("All candidate profile IDs must be valid (non-empty).")
                .Must(HasNoDuplicates)
                    .WithMessage("Duplicate candidate selections are not allowed. Each candidate can only be selected once.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .NotEqual(Guid.Empty).WithMessage("National ID cannot be empty.");

            RuleFor(x => x.VotingToken)
                .NotEmpty().WithMessage("Voting token is required.")
                .NotEqual(Guid.Empty).WithMessage("Voting token cannot be empty.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required for vote confirmation.");
        }

        private static bool HasNoDuplicates(List<Guid>? ids)
        {
            if (ids == null || ids.Count == 0)
                return true; // Other rules will handle null/empty

            return ids.Distinct().Count() == ids.Count;
        }
    }
}
