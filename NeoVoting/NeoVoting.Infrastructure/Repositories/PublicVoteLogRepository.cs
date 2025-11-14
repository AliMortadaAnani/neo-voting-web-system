using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
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

        public async Task<PublicVoteLog?> GetPublicVoteLogByIdAsync(long logId, CancellationToken cancellationToken)
        {
            return await _dbContext.PublicVoteLogs
                .Include(l => l.Vote)
                .Include(l => l.Election)
                .Include(l => l.Governorate)
                .FirstOrDefaultAsync(l => l.Id == logId, cancellationToken);
        }

        public async Task<List<PublicVoteLog>> GetAllPublicVoteLogsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.PublicVoteLogs
                .Include(l => l.Vote)
                .Include(l => l.Election)
                .Include(l => l.Governorate)
                .ToListAsync(cancellationToken);
        }

        public async Task<PublicVoteLog> AddPublicVoteLogAsync(PublicVoteLog log, CancellationToken cancellationToken)
        {
            await _dbContext.PublicVoteLogs.AddAsync(log, cancellationToken);
            return log;
        }

        
    }
}