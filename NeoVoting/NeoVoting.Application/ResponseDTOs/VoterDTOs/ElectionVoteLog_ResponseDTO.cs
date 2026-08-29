using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.VoterDTOs
{
    public class ElectionVoteLog_ResponseDTO
    {
        public Guid? VoteId { get; set; }
        public int? ElectionId { get; set; }
        public string? ElectionName { get; set; }
        public DateTime? TimestampUTC { get; set; }
        public GovernorateIdEnum? GovernorateId { get; set; }
    }
}