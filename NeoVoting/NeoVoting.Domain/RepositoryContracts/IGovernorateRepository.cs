using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IGovernorateRepository
    {
        // TODO: Consider caching these instances if they are frequently used.
        Task<IReadOnlyList<Governorate>> GetAllGovernoratesAsync(CancellationToken cancellationToken);
    }
}
