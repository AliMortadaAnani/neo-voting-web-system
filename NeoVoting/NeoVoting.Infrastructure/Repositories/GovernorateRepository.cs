using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;

namespace NeoVoting.Infrastructure.Repositories
{
    public class GovernorateRepository : IGovernorateRepository
    {
        // Implementation will be added later
        public Task<IReadOnlyList<Governorate>> GetAllGovernorates()
        {
            throw new NotImplementedException();
        }
    }
}