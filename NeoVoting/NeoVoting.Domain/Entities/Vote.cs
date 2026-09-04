using NeoVoting.Domain.Enums; // Assuming enums for Governorates are here

namespace NeoVoting.Domain.Entities
{
    /// <summary>
    /// Represents a single, immutable vote cast in an election.

    /// </summary>
    public class Vote
    {
        // --- Properties ---

        public Guid Id { get; private set; }
        public DateTime TimestampUTC { get; private set; }

        // --- Foreign Keys & Navigation Properties ---

        public int ElectionId { get; private set; }
        public Election Election { get; private set; }

        public int CandidateProfileId { get; private set; }
        public CandidateProfile CandidateProfile { get; private set; }



        private Vote()
        {
            Election = null!;
            CandidateProfile = null!;
        }

        public static Vote Create(int electionId, int candidateProfileId)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                ElectionId = electionId,
                CandidateProfileId = candidateProfileId,
                TimestampUTC = DateTime.UtcNow // The timestamp is always set at the moment of creation.
            };

            return vote;
        }

        // A Vote, once created, is immutable. There are no "Update" methods.
    }
}