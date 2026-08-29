using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.AuthDTOs
{
    // response dto dont need to be nullable to avoid .Net 8 nullable warnings when validating since they are only used for output
    //otherwise, we need to make every request dto property nullable to avoid the warnings and rely only on FluentValidation for validation
    public class Authentication_ResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

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
    }
}