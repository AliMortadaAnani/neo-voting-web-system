namespace NeoVoting.Application.NeoVotingDTOs
{
    public class NeoVoting_VoterIsRegisteredRequestDTO
    {
        public Guid? NationalId { get; set; }
        public Guid? VotingToken { get; set; }

        public string? RegisteredUsername { get; set; }
    }
}