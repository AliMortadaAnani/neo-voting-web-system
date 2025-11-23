namespace GovernmentSystem.API.Application.RequestDTOs
{
    public class NeoVoting_VoterIsRegisteredRequestDTO
    {
        public Guid? NationalId { get; set; }
        public Guid? VotingToken { get; set; }
    }
}