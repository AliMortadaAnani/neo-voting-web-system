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
        public Guid VoteId { get; private set; }
        public DateTime TimestampUTC { get; private set; }
        public int ElectionId { get; private set; }
        public string ElectionName { get; private set; } = string.Empty;
        public GovernorateIdEnum Governorate { get; private set; }

        private PublicVoteLog() { }

        // --- Factory Method ---

        /// <summary>
        /// Creates a new PublicVoteLog instance to record the occurrence of a vote.
        /// </summary>
        public static PublicVoteLog Create(
            Guid voteId,
            int electionId,
            GovernorateIdEnum governorate,
            string electionName
            )
        {
            return new PublicVoteLog
            {
                VoteId = voteId,
                ElectionId = electionId,
                ElectionName = electionName,
                Governorate = governorate,
                TimestampUTC = DateTime.UtcNow
            };
        }

       
    }
}