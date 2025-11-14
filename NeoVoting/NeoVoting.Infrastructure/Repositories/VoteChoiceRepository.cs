using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteChoiceRepository : IVoteChoiceRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public VoteChoiceRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Implementation will be added later
    }
}