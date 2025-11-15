using NeoVoting.Domain.Entities;

namespace NeoVoting.Application.RequestDTOs
{
    /// <summary>
    /// Represents the data required to cast a new vote.
    /// </summary>
    public class VoteAddRequest
    {
        public Guid ElectionId { get; set; }
        public int GovernorateId { get; set; }
        public int VoterAge { get; set; }
        public char VoterGender { get; set; }



        /// <summary>
        /// Maps the core vote data to a new Vote domain entity.
        /// Note: The choices are handled separately by the service.
        /// </summary>
        /// <returns>A new, validated Vote entity.</returns>
        public Vote ToVote()
        {
            return Vote.Create(
                ElectionId,
                GovernorateId,
                VoterAge,
                VoterGender
            );
        }
    }
}