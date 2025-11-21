using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.Application.ResponseDTOs
{
    public class VoterResponseDTO
    {
        public Guid Id { get; set; }
        public Guid NationalId { get; set; }
        public Guid VotingToken { get; set; }
        public GovernorateId GovernorateId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public char Gender { get; set; }
        public bool EligibleForElection { get; set; }
        public bool ValidToken { get; set; }
        public bool IsRegistered { get; set; }
        public bool Voted { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(VoterResponseDTO)) return false;

            VoterResponseDTO other = (VoterResponseDTO)obj;

            return this.Id == other.Id &&
                   this.NationalId == other.NationalId &&
                   this.VotingToken == other.VotingToken &&
                   this.GovernorateId == other.GovernorateId &&
                   this.FirstName == other.FirstName &&
                   this.LastName == other.LastName &&
                   this.DateOfBirth == other.DateOfBirth &&
                   this.Gender == other.Gender &&
                   this.EligibleForElection == other.EligibleForElection &&
                   this.ValidToken == other.ValidToken &&
                   this.IsRegistered == other.IsRegistered &&
                   this.Voted == other.Voted;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}