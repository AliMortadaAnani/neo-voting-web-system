namespace NeoVoting.Domain.Entities
{
    /// <summary>
    /// Represents the link between a single Vote and a single CandidateProfile.
    /// A collection of these objects represents a single voter's choices.
    /// </summary>
    public class VoteChoice
    {
        // --- Properties ---

        public int Id { get; private set; }

        // --- Foreign Keys & Navigation Properties ---

        public Guid VoteId { get; private set; }
        public Vote Vote { get; private set; }

        public int CandidateProfileId { get; private set; }
        public CandidateProfile CandidateProfile { get; private set; }

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
        public static VoteChoice Create(Guid voteId, int candidateProfileId)
        {
            var voteChoice = new VoteChoice
            {
                VoteId = voteId,
                CandidateProfileId = candidateProfileId
            };

            return voteChoice;
        }

        // Like a Vote, a VoteChoice is immutable. Once created, it should not be changed.
    }
}