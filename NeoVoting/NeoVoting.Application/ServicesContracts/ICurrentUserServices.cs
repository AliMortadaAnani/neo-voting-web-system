namespace NeoVoting.Application.ServicesContracts
{
    public interface ICurrentUserServices
    {
        Guid? UserId { get; }
        string? Username { get; }

        Guid GetAuthenticatedUserId();

        // Make sure you added this line:
        string GetAuthenticatedUsername();
    }
}