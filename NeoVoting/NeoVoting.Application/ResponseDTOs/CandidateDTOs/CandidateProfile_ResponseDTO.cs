using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.CandidateDTOs
{
    public class CandidateProfile_ResponseDTO
    {
        public int? Id { get; set; }
        public string? Goals { get; set; }
        public string? NominationReasons { get; set; }
        public string? ProfilePhotoFilename { get; set; }
        public int? ApplicationUserId { get; set; }
        public int? CandidateId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public char? Gender { get; set; }
        public GovernorateIdEnum? Governorate { get; set; }
        public int? ElectionId { get; set; }
        public string? ElectionName { get; set; }
    }
}