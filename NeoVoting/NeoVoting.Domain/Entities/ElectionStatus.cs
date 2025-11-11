using NeoVoting.Domain.Enums; 

namespace NeoVoting.Domain.Entities
{
    public class ElectionStatus
    {   // TODO: Consider caching these instances if they are frequently used.
        // --- Properties ---

        public int Id { get; private set; }
        public required string Name { get; init; }

        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all object creation through the controlled,
        /// static factory methods. EF Core uses this for materializing from the DB.
        /// </summary>
        private ElectionStatus() { }

        // --- ToString() Override ---

        /// <summary>
        /// Provides a meaningful string representation for debugging and logging.
        /// </summary>
        /// <returns>A string like "Voting (Id: 3)".</returns>
        public override string ToString()
        {
            return $"{Name} (Id: {Id})";
        }


        // --- Factory Methods (Dependent on the Enum) ---

        /// <summary>
        /// Creates a new instance representing the Upcoming status.
        /// </summary>
        public static ElectionStatus CreateUpcomingStatus()
        {
            return new ElectionStatus
            {
                Id = (int)ElectionStatusEnum.Upcoming,
                Name = ElectionStatusEnum.Upcoming.GetDescription()
            };
        }

        /// <summary>
        /// Creates a new instance representing the Nomination status.
        /// </summary>
        public static ElectionStatus CreateNominationStatus()
        {
            return new ElectionStatus
            {
                Id = (int)ElectionStatusEnum.Nomination,
                Name = ElectionStatusEnum.Nomination.GetDescription()
            };
        }

        /// <summary>
        /// Creates a new instance representing the Voting status.
        /// </summary>
        public static ElectionStatus CreateVotingStatus()
        {
            return new ElectionStatus
            {
                Id = (int)ElectionStatusEnum.Voting,
                Name = ElectionStatusEnum.Voting.GetDescription()
            };
        }

        /// <summary>
        /// Creates a new instance representing the Completed status.
        /// </summary>
        public static ElectionStatus CreateCompletedStatus()
        {
            return new ElectionStatus
            {
                Id = (int)ElectionStatusEnum.Completed,
                Name = ElectionStatusEnum.Completed.GetDescription()
            };
        }
    }
}
