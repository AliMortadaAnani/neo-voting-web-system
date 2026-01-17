using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IGovernorateRepository
    {
        // for testing purposes
        Task<IReadOnlyList<Governorate>> GetAllGovernoratesAsync(CancellationToken cancellationToken);
    }
}