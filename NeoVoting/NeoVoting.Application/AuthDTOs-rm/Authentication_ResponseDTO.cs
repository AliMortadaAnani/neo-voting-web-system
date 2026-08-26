namespace NeoVoting.Application.AuthDTOs
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
        public Guid? Id { get; set; }

        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public int? GovernorateId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public char? Gender { get; set; }
        public string? Role { get; set; }
    }
}