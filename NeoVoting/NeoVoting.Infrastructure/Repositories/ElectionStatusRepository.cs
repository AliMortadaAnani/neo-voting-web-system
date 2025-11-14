using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionStatusRepository : IElectionStatusRepository
    {
        // Implementation will be added later
        public Task<IReadOnlyList<ElectionStatus>> GetAllElectionStatuses()
        {
            throw new NotImplementedException();
        }
    }
}