using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.ResponseDTOs
{
    public class NeoVoting_CandidateResponseDTO
    {
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