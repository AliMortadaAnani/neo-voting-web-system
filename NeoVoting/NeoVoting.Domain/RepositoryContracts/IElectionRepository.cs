using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionRepository
    {

        Task<Election> AddElectionAsync(Election election, CancellationToken cancellationToken);

        Task<bool> IsActiveElectionExistsAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<Election>> GetAllElectionsAsync(CancellationToken cancellationToken);

        Task<Election?> GetElectionByIdAsync(Guid electionId, CancellationToken cancellationToken);

        Task<Election?> GetLastUpcomingOrActiveElectionAsync(CancellationToken cancellationToken);

        void Update(Election election);




    }

}