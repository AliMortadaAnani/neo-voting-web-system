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

        public async Task<PublicVoteLog> AddPublicVoteLogAsync(PublicVoteLog log, CancellationToken cancellationToken)
        {
            await _dbContext.PublicVoteLogs.AddAsync(log, cancellationToken);
            return log;
        }

        public async Task<IReadOnlyList<PublicVoteLog>> GetAllPublicVoteLogsByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.PublicVoteLogs
                
                .Where(l => l.ElectionId == ElectionId)
                .OrderByDescending(l => l.TimestampUTC)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<PublicVoteLog>> GetPagedPublicVoteLogsByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<PublicVoteLog>> GetPagedPublicVoteLogsByElectionIdAsync(Guid ElectionId, int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.PublicVoteLogs
                
                .Where(l => l.ElectionId == ElectionId)
                .OrderByDescending(l => l.TimestampUTC)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<PublicVoteLog?> GetPublicVoteLogByVoteIdAsync(Guid VoteId, CancellationToken cancellationToken)
        {
            return await _dbContext.PublicVoteLogs
               
                .FirstOrDefaultAsync(l => l.VoteId == VoteId, cancellationToken);
        }

        public Task<int> GetTotalPublicVoteLogsCountByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetTotalPublicVoteLogsCountByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            return await _dbContext.PublicVoteLogs
                .CountAsync(l => l.ElectionId == electionId, cancellationToken);
        }
    }
}