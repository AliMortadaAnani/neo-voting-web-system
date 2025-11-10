using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoriesContracts
{
    public interface IElectionStatusRepository
    {
        Task<IReadOnlyList<ElectionStatus>> GetAllElectionStatuses();
    }
}
