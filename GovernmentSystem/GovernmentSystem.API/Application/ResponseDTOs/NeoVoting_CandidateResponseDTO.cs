using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.Application.ResponseDTOs
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

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(NeoVoting_CandidateResponseDTO)) return false;

            NeoVoting_CandidateResponseDTO other = (NeoVoting_CandidateResponseDTO)obj;

            return this.GovernorateId == other.GovernorateId &&
                   this.FirstName == other.FirstName &&
                   this.LastName == other.LastName &&
                   this.DateOfBirth == other.DateOfBirth &&
                   this.Gender == other.Gender &&
                   this.EligibleForElection == other.EligibleForElection &&
                   this.ValidToken == other.ValidToken &&
                   this.IsRegistered == other.IsRegistered;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}