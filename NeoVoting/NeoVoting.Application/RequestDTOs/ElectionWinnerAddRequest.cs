using NeoVoting.Domain.Entities;

namespace NeoVoting.Application.RequestDTOs
{
    /// <summary>
    /// Represents the data required to declare a new winner for an election.
    /// </summary>
    public class ElectionWinnerAddRequest
    {
        public Guid ElectionId { get; set; }
        public Guid CandidateProfileId { get; set; }
        public int? VoteCount { get; set; }

        /// <summary>
        /// Maps this DTO to a new ElectionWinner domain entity.
        /// </summary>
        /// <returns>A new, validated ElectionWinner entity.</returns>
        public ElectionWinner ToElectionWinner()
        {
            return ElectionWinner.Create(
                CandidateProfileId,
                VoteCount
            );
        }
    }
}