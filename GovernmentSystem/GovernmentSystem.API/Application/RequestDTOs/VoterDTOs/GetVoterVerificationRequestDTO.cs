namespace GovernmentSystem.API.Application.RequestDTOs.VoterDTOs
{
    public class GetVoterVerificationRequestDTO
    {
        public string? NationalId { get; set; }

        public string? VotingToken { get; set; }
    }
}