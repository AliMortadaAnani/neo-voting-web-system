using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.RequestDTOs
{
    public class CreateCandidateRequestDTO
    {
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }   
        public GovernorateId? GovernorateId { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public char? Gender { get; set; }
        public bool? EligibleForElection { get; set; }

        // Mapping Method
        public Candidate ToCandidate()
        {
            return Candidate.Create(
                (GovernorateId)GovernorateId!.Value,
                FirstName!,
                LastName!,
                DateOfBirth!.Value,
                Gender!.Value,
                EligibleForElection!.Value
            );
        }
    }
}