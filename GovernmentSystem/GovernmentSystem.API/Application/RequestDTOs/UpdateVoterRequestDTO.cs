using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.Application.RequestDTOs
{
    public class UpdateVoterRequestDTO
    {
        // We pass NationalId in the URL or wrapper, but usually update bodies contain data

        public Guid NationalId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public GovernorateId GovernorateId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public char Gender { get; set; }
        public bool EligibleForElection { get; set; }
        public bool ValidToken { get; set; }
        public bool IsRegistered { get; set; }
        public bool Voted { get; set; }
    }
}