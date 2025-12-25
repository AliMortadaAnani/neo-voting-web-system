using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionRepository
    {
        Task<IReadOnlyList<Election>> GetAllElectionsAsync(CancellationToken cancellationToken);

        Task<Election?> GetElectionByIdAsync(Guid ElectionId, CancellationToken cancellationToken);

        Task<Election> AddElectionAsync(Election election, CancellationToken cancellationToken);

        void Update(Election election);

        Task<Election?> GetLastCompletedElectionAsync(CancellationToken cancellationToken);

        Task<Election?> GetLastUpcomingOrActiveElectionAsync(CancellationToken cancellationToken);

        Task<int> GetRegisteredVotersCountByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);


    }
}