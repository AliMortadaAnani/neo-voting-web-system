namespace NeoVoting.Application.ResponseDTOs.VoterDTOs
{
    public class ElectionVoteLog_ResponseDTO
    {
        public Guid? VoteId { get; set; }
        public DateTime? TimestampUTC { get; set; }
    }
}