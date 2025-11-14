using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
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

        public async Task<ElectionWinner?> GetWinnerByIdAsync(int winnerId, CancellationToken cancellationToken)
        {
            return await _dbContext.ElectionWinners
                .Include(w => w.Election)
                .Include(w => w.CandidateProfile)
                .FirstOrDefaultAsync(w => w.Id == winnerId, cancellationToken);
        }

        public async Task<List<ElectionWinner>> GetAllWinnersAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.ElectionWinners
                .Include(w => w.Election)
                .Include(w => w.CandidateProfile)
                .ToListAsync(cancellationToken);
        }

        public async Task<ElectionWinner> AddWinnerAsync(ElectionWinner winner, CancellationToken cancellationToken)
        {
            await _dbContext.ElectionWinners.AddAsync(winner, cancellationToken);
            return winner;
        }

        public void Update(ElectionWinner winner)
        {
            _dbContext.ElectionWinners.Update(winner);
        }
    }
}