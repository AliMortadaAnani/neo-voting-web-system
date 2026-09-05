namespace NeoVoting.Application.ServicesContracts
{
    public interface ICurrentUserServices
    {
        int? ApplicationUserId { get; }
        int? AccountId { get; }
        string? UserName { get; }
    }
}