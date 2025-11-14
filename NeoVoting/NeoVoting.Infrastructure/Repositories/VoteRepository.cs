using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public VoteRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Implementation will be added later
    }
}