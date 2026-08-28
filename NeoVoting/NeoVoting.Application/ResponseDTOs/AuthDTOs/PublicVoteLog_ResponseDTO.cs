namespace NeoVoting.Application.ResponseDTOs
{
    public class PublicVoteLog_ResponseDTO
    {
        public long Id { get; set; }
        public DateTime TimestampUTC { get; set; }
        public string? ErrorMessage { get; set; }

        public Guid VoteId { get; set; }
        public Guid ElectionId { get; set; }
        public int GovernorateId { get; set; }
        public string GovernorateName { get; set; } = string.Empty;
        public string ElectionName { get; set; } = string.Empty;
    }
}