namespace GovernmentSystem.Application.RequestDTOs
{
    public class NeoVoting_GetVoterRequestDTO
    {
        public Guid NationalId { get; set; }
        public Guid VotingToken { get; set; }
    }
}