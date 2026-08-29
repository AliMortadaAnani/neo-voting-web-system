using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.AdminDTOs
{
    public class Election_ResponseDTO
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? NominationStartDate { get; set; }
        public DateTime? NominationEndDate { get; set; }
        public DateTime? VotingStartDate { get; set; }
        public DateTime? VotingEndDate { get; set; }

        public StatusEnum Status { get; set; }
    }
}