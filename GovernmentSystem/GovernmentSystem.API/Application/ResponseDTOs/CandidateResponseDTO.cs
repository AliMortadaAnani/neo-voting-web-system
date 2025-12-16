using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.ResponseDTOs
{
    public class CandidateResponseDTO
    {
        public Guid Id { get; set; }
        public Guid NationalId { get; set; }
        public Guid NominationToken { get; set; }
        public GovernorateId GovernorateId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public char Gender { get; set; }
        public bool EligibleForElection { get; set; }
        public bool ValidToken { get; set; }
        public bool IsRegistered { get; set; }

        
    }
}