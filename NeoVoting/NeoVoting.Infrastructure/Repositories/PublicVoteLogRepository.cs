using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class PublicVoteLogRepository : IPublicVoteLogRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public PublicVoteLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Implementation will be added later
    }
}