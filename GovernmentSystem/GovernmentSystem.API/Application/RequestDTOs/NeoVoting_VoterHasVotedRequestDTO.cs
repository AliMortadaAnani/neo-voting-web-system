namespace GovernmentSystem.Application.RequestDTOs
{
    public class NeoVoting_VoterHasVotedRequestDTO
    {
        public Guid NationalId { get; set; }
        public Guid VotingToken { get; set; }
    }
}