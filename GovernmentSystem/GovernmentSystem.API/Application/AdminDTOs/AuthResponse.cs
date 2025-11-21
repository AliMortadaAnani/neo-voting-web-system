namespace GovernmentSystem.API.Application.AdminDTOs
{
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        // Useful for the Frontend UI
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;// e.g., "Admin"
    }
}
