using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.ResponseDTOs
{
    public class NeoVoting_VoterResponseDTO
    {
        // this is the response that external callers should get
        // so we only return non sensitive information
        //other wise our internal VoterResponseDTO contains sensitive information for internal use only in GovernmentSystem by authorized personal (Admin)
        public GovernorateId GovernorateId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public char Gender { get; set; }
        public bool EligibleForElection { get; set; }
        public bool ValidToken { get; set; } // Boolean status only
        public bool IsRegistered { get; set; }
        public bool Voted { get; set; }
    }
}