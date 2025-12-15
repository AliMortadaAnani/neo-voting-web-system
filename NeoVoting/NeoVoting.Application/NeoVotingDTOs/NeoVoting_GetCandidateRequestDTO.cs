namespace NeoVoting.Application.NeoVotingDTOs
{
    public class NeoVoting_GetCandidateRequestDTO
    {
        public Guid? NationalId { get; set; }
        public Guid? NominationToken { get; set; }
    }
}