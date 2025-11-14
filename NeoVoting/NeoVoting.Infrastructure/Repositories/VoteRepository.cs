using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
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

        public async Task<Vote?> GetVoteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Votes
                .Include(v => v.Election)
                .Include(v => v.Governorate)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        }

        public async Task<List<Vote>> GetAllVotesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Votes
                .Include(v => v.Election)
                .Include(v => v.Governorate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken)
        {
            await _dbContext.Votes.AddAsync(vote, cancellationToken);
            return vote;
        }

        
    }
}