namespace NeoVoting.Application.AuthDTOs
{
    public class Registration_ResetPassword_ResponseDTO
    {
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