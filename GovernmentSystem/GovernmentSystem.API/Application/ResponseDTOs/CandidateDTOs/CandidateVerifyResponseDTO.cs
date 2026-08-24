using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs
{
    public class CandidateVerifyResponseDTO
    {
        public string HashedData { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public GovernorateIdEnum Governorate { get; set; }
        public char Gender { get; set; }
    }
}