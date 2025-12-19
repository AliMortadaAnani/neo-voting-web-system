using System.Collections.Generic;
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

        public async Task<IReadOnlyList<ElectionWinner>> GetAllWinnersByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.ElectionWinners
                .Where(w => w.ElectionId == ElectionId)
                .OrderByDescending(w => w.VoteCount)
                .ThenBy(w => w.CandidateProfile.User.UserName)
                .Include(w => w.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(w => w.Election)
                .AsNoTracking()
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