using NeoVoting.Domain.Entities;

namespace NeoVoting.Application.RequestDTOs
{
    // <summary>
    /// Represents the data required to create a new election.
    /// </summary>
    public class ElectionAddRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime NominationStartDate { get; set; }
        public DateTime NominationEndDate { get; set; }
        public DateTime VotingStartDate { get; set; }
        public DateTime VotingEndDate { get; set; }

        public Election ToElection()
        {
            return Election.Create(
                Name,
                NominationStartDate,
                NominationEndDate,
                VotingStartDate,
                VotingEndDate
            );
        }
    }
}
