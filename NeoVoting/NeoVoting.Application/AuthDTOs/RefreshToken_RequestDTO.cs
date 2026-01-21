namespace NeoVoting.Application.AuthDTOs
{
    public class RefreshToken_RequestDTO
    {
        public string? RefreshToken { get; set; }

        public string? AccessToken { get; set; }
    }
}