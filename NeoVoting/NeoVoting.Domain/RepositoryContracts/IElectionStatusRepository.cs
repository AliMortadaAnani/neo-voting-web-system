using NeoVoting.Domain.Entities;
using System.Threading;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionStatusRepository
    {
        // TODO: Consider caching these instances if they are frequently used.
        Task<IReadOnlyList<ElectionStatus>> GetAllElectionStatusesAsync(CancellationToken cancellationToken);
    }
}
