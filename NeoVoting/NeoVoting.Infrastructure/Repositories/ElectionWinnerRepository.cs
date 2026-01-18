using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionWinnerRepository(ApplicationDbContext dbContext) : IElectionWinnerRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IReadOnlyList<ElectionWinner>> GetAllWinnersByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.ElectionWinners
                .Include(w => w.CandidateProfile)
                    .ThenInclude(cp => cp.User)
                .Where(w => w.CandidateProfile.ElectionId == ElectionId)
                .OrderByDescending(w => w.VoteCount)
                    .ThenBy(w => w.CandidateProfile.User.UserName)
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

        public Task<IReadOnlyList<ElectionWinner>> GetAllWinnersByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}