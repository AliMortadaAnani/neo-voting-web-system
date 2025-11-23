using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.RequestDTOs
{
    public class UpdateVoterRequestDTO
    {
        public Guid? NationalId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public GovernorateId? GovernorateId { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public char? Gender { get; set; }
        public bool? EligibleForElection { get; set; }
        public bool? ValidToken { get; set; }
        public bool? IsRegistered { get; set; }
        public bool? Voted { get; set; }
    }
}