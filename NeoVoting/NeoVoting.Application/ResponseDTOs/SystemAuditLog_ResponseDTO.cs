using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ResponseDTOs
{
    public class SystemAuditLog_ResponseDTO
    {
        public long Id { get; set; }
        public DateTime TimestampUTC { get; set; }
        public SystemActionTypesEnum ActionType { get; set; }
        public string? Details { get; set; }
        public Guid UserId { get; set; }

        // Snapshots
        public string Username { get; set; } = string.Empty;
        public string? ElectionName { get; set; }

        // Links
        public Guid? CandidateProfileId { get; set; }
        public Guid? ElectionId { get; set; }
    }
}
