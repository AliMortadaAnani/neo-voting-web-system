using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    /// <summary>
    /// Represents an immutable, publicly visible log entry for a single vote cast.
    /// This is designed for transparency and auditing, recording the event of a vote
    /// without linking to the voter or their specific choices.
    /// </summary>
    public class PublicVoteLog
    {
        public long Id { get; private set; }
        public DateTime TimestampUTC { get; private set; }
        public string? ErrorMessage { get; private set; }

        public Guid VoteId { get; private set; }
        public Guid ElectionId { get; private set; }
        public int GovernorateId { get; private set; }
        public string GovernorateName { get; private set; } = string.Empty;
        public string ElectionName { get; private set; } = string.Empty;

        private PublicVoteLog() { }

        // --- Factory Method ---

        /// <summary>
        /// Creates a new PublicVoteLog instance to record the occurrence of a vote.
        /// </summary>
        public static PublicVoteLog Create(
            Guid voteId,
            Guid electionId,
            int governorateId,
            string governorateName,
            string electionName,
            string? errorMessage)
        {
            Validate(voteId, electionId, governorateId, governorateName, electionName);

            return new PublicVoteLog
            {
                VoteId = voteId,
                ElectionId = electionId,
                GovernorateId = governorateId,
                GovernorateName = governorateName,
                ElectionName = electionName,
                ErrorMessage = errorMessage,
                TimestampUTC = DateTime.UtcNow
            };
        }

        private static void Validate(Guid voteId, Guid electionId, int governorateId, string governorateName, string electionName)
        {
            if (voteId == Guid.Empty)
                throw new ArgumentException("VoteId cannot be empty.", nameof(voteId));

            if (electionId == Guid.Empty)
                throw new ArgumentException("ElectionId cannot be empty.", nameof(electionId));

            if (!Enum.IsDefined(typeof(GovernoratesEnum), governorateId))
                throw new ArgumentException("GovernorateId must be valid.", nameof(governorateId));

            if (string.IsNullOrWhiteSpace(governorateName))
                throw new ArgumentException("GovernorateName is required.", nameof(governorateName));

            if (string.IsNullOrWhiteSpace(electionName))
                throw new ArgumentException("ElectionName is required.", nameof(electionName));
        }
    }
}