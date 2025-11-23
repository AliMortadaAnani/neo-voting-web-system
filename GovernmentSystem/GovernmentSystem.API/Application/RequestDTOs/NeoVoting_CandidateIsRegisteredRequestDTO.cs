namespace GovernmentSystem.API.Application.RequestDTOs
{
    public class NeoVoting_CandidateIsRegisteredRequestDTO
    {
        public Guid? NationalId { get; set; }
        public Guid? NominationToken { get; set; }
    }
}