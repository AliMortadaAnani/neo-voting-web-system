using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class ElectionStatus
    {
        public int Id { get; private set; }
        public required string Name { get; init; }

        private ElectionStatus()
        { }

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
        /// Creates a new instance representing the pre-Voting status,which is after nomination ends and before voting starts.
        /// </summary>
        public static ElectionStatus CreatePreVotingStatus()
        {
            return new ElectionStatus
            {
                Id = (int)ElectionStatusEnum.PreVotingPhase,
                Name = ElectionStatusEnum.PreVotingPhase.GetDescription()
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