namespace NeoVoting.Application.RequestDTOs.AuthDTOs
{
    public class RefreshToken_RequestDTO
    {
        public string? RefreshToken { get; set; } = string.Empty; // we will try to extract the refresh token from the cookie if not provided in the request body, but we will still allow it to be provided in the request body for flexibility

        public string? AccessToken { get; set; } = string.Empty; // from headers

    }
}