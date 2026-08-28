namespace NeoVoting.Domain.Entities
{
    public class PollVote
    {
        public Guid Id { get; private set; }

        public DateTime TimestampUTC { get; private set; }

        // --- Foreign Keys & Navigation Properties ---

        public int PollId { get; private set; }
        public Poll Poll { get; private set; }

        public int PollAnswerId { get; private set; }
        public PollAnswer PollAnswer { get; private set; }

        private PollVote()
        {
            Poll = null!;
            PollAnswer = null!;
        }

        public static PollVote Create(int pollId, int pollAnswerId)
        {
            var pollVote = new PollVote
            {
                Id = Guid.NewGuid(),
                PollId = pollId,
                PollAnswerId = pollAnswerId,

                TimestampUTC = DateTime.UtcNow // The timestamp is always set at the moment of creation.
            };

            return pollVote;
        }

        // A PollVote, once created, is immutable. There are no "Update" methods.
    }
}