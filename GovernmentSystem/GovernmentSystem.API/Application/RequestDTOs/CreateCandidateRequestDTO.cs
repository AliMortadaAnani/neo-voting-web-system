using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.Application.RequestDTOs
{
    public class CreateCandidateRequestDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public GovernorateId GovernorateId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public char Gender { get; set; }
        public bool EligibleForElection { get; set; }

        // Mapping Method
        public Candidate ToCandidate()
        {
            return Candidate.Create(
                GovernorateId,
                FirstName,
                LastName,
                DateOfBirth,
                Gender,
                EligibleForElection
            );
        }
    }
}