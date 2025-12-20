namespace NeoVoting.Application.AuthDTOs
{
    public class RefreshTokenRequestDTO
    {
        public string? RefreshToken { get; set; }

        public string? AccessToken { get; set; }
    }
}