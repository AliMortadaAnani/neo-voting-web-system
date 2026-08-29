using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.AdminDTOs
{
    public class SystemAuditLog_ResponseDTO
    {
        public long? Id { get; set; }
        public DateTime? TimestampUTC { get; set; }
        public SystemActionTypesEnum? ActionType { get; set; }
        public string? Details { get; set; } // will contain a small description of the action, e.g., "Election created with ID: {electionId}" ....
        public int? AdminId { get; set; }
        public string? Username { get; set; }
    }
}