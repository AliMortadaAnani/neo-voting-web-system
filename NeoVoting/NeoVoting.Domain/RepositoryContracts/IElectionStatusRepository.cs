using System.Collections.Generic;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionStatusRepository
    {
        
        Task<IReadOnlyList<ElectionStatus>> GetAllElectionStatusesAsync(CancellationToken cancellationToken);
    }
}
