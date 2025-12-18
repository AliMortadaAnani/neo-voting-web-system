using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionRepository
    {
        
        Task<List<Election>> GetAllElectionsAsync(CancellationToken cancellationToken);

        Task<Election?> GetElectionByIdAsync(Guid ElectionId, CancellationToken cancellationToken);

        Task<Election> AddElectionAsync(Election election, CancellationToken cancellationToken);


        void Update(Election election);




    }
}
