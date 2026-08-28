namespace NeoVoting.Application.ResponseDTOs
{
    public class VoterCastVote_ResponseDTO
    {
        public Guid VoteId { get; set; }
        public Guid? ElectionId { get; set; }
        public string? ElectionName { get; set; }
        public int? GovernorateId { get; set; }
        public string? GovernorateName { get; set; }
    }
}