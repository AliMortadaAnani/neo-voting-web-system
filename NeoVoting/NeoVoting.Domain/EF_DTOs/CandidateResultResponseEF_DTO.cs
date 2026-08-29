using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.EF_DTOs
{
    public class CandidateResultResponseEF_DTO
    {
        public int CandidateProfileId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public GovernorateIdEnum Governorate { get; set; }
        public string ProfilePhotoFilename { get; set; } = string.Empty;
        public int VoteCount { get; set; }
    }
}