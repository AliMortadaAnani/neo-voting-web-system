namespace NeoVoting.Application.ResponseDTOs.VoterDTOs
{
    public class PollVoteLog_ResponseDTO
    {
        public Guid? VoteId { get; set; }
        public int? PollId { get; set; }
        public string? PollName { get; set; }
        public DateTime? TimestampUTC { get; set; }
    }
}