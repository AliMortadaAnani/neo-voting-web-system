using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums; // Don't forget to import the enum's namespace

namespace NeoVoting.Domain.Entities
{
    public class ElectionStatus
    {
        // --- Properties ---

        public int Id { get;  set; }
        public string Name { get; set; }

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
        public static ElectionStatus CreateUpcoming()
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
        public static ElectionStatus CreateNomination()
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
        public static ElectionStatus CreateVoting()
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
        public static ElectionStatus CreateCompleted()
        {
            return new ElectionStatus
            {
                Id = (int)ElectionStatusEnum.Completed,
                Name = ElectionStatusEnum.Completed.GetDescription()
            };
        }
    }
}
