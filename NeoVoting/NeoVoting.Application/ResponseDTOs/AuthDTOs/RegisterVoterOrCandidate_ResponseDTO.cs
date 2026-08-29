using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.AuthDTOs
{
    public class RegisterVoterOrCandidate_ResponseDTO
    {
        // --- User Info (For the UI) ---
        public int? ApplicationUserId { get; set; }

        public int? AccountId { get; set; } // voter or candidate account id
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public GovernorateIdEnum? Governorate { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public char? Gender { get; set; }
        public RoleTypesEnum? Role { get; set; }

        public string? Message { get; set; } = "User registered successfully, please login to continue.";
    }
}