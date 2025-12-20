using NeoVoting.Domain.Contracts;
using System.Text;

namespace NeoVoting.Domain.Entities
{
    /// <summary>
    /// Represents the link between a single Vote and a single CandidateProfile.
    /// A collection of these objects represents a single voter's choices.
    /// </summary>
    public class VoteChoice : ISoftDeletable
    {
        // --- Properties ---

        public Guid Id { get; private set; }

        // --- Foreign Keys & Navigation Properties ---

        public Guid VoteId { get; private set; }
        public Vote Vote { get; private set; }

        public Guid CandidateProfileId { get; private set; }
        public CandidateProfile CandidateProfile { get; private set; }

        // --- Soft Delete Implementation ---
        public bool IsDeleted { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        private VoteChoice()
        {
            Vote = null!;
            CandidateProfile = null!;
        }

        // --- Factory Method ---

        /// <summary>
        /// Creates a new, valid VoteChoice instance.
        /// </summary>
        /// <param name="voteId">The ID of the parent vote.</param>
        /// <param name="candidateProfileId">The ID of the candidate being voted for.</param>
        /// <returns>A new, valid VoteChoice object.</returns>
        /// <exception cref="ArgumentException">Thrown if validation fails.</exception>
        public static VoteChoice Create(Guid voteId, Guid candidateProfileId)
        {
            Validate(voteId, candidateProfileId);

            var voteChoice = new VoteChoice
            {
                Id = Guid.NewGuid(),
                VoteId = voteId,
                CandidateProfileId = candidateProfileId
            };

            return voteChoice;
        }

        // Like a Vote, a VoteChoice is immutable. Once created, it should not be changed.

        /// <summary>
        //  Private helper method for validating the foreign keys.
        /// </summary>
        private static void Validate(Guid voteId, Guid candidateProfileId)
        {
            var errors = new StringBuilder();

            if (voteId == Guid.Empty)
            {
                errors.AppendLine("VoteId is required.");
            }

            if (candidateProfileId == Guid.Empty)
            {
                errors.AppendLine("CandidateProfileId is required.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }
    }
}