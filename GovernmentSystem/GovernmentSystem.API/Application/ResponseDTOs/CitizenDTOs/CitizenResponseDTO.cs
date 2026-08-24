using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs
{
    public class CitizenResponseDTO
    {
        public int Id { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public GovernorateIdEnum GovernorateId { get; set; }
        public char Gender { get; set; }
    }
}