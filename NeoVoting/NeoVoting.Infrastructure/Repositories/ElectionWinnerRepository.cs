using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionWinnerRepository : IElectionWinnerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ElectionWinnerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Implementation will be added later
    }
}