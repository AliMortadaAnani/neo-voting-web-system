using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs
{
    public class UpdateCitizenRequestDTO
    {
        public string? NationalId { get; set; } // to find the citizen to update

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public GovernorateIdEnum? GovernorateId { get; set; }

        public char? Gender { get; set; }
    }
}