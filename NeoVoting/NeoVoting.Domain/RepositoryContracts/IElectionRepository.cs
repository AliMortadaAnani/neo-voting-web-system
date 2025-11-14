using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionRepository
    {
        // Example method with CancellationToken
        // Task<Election?> GetElectionByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<List<Election>> GetAllElectionsAsync(CancellationToken cancellationToken);

        Task<Election?> GetElectionByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<Election> AddElectionAsync(Election election, CancellationToken cancellationToken);


        // Add these back. They are simple, synchronous persistence operations.
        void Update(Election election); 


        //Application logic moved to IElectionService
        /*Task<Election?> UpdateElectionDetailsAsync(Election election, CancellationToken cancellationToken);

        Task<Election?> UpdateElectionStatusAsync(Election election,ElectionStatusEnum updatedElectionStatus,CancellationToken cancellationToken);*/



    }
}
