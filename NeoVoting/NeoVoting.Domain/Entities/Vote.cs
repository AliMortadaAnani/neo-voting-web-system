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
        public int VoterAge { get; private set; }
        public char VoterGender { get; private set; }
        public GovernorateIdEnum Governorate { get; private set; }
        public DateTime TimestampUTC { get; private set; }

        // --- Foreign Keys & Navigation Properties ---

        public int ElectionId { get; private set; }
        public Election Election { get; private set; }

        public ICollection<VoteChoice> VoteChoices { get; private set; } = new List<VoteChoice>();

        private Vote()
        {
            Election = null!;
        }

        public static Vote Create(int electionId, GovernorateIdEnum governorate, int voterAge, char voterGender)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                ElectionId = electionId,
                Governorate = governorate,
                VoterAge = voterAge,
                VoterGender = char.ToUpper(voterGender),
                TimestampUTC = DateTime.UtcNow // The timestamp is always set at the moment of creation.
            };

            return vote;
        }

        // A Vote, once created, is immutable. There are no "Update" methods.
    }
}