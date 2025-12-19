using System.Collections.Generic;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionRepository
    {
        
        Task<IReadOnlyList<Election>> GetAllElectionsAsync(CancellationToken cancellationToken);

        Task<Election?> GetElectionByIdAsync(Guid ElectionId, CancellationToken cancellationToken);

        Task<Election> AddElectionAsync(Election election, CancellationToken cancellationToken);


        void Update(Election election);




    }
}
