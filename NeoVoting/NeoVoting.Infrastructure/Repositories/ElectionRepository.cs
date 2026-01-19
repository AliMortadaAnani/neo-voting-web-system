using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionRepository(ApplicationDbContext dbContext) : IElectionRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Election> AddElectionAsync(Election election, CancellationToken cancellationToken)
        {
            await _dbContext.Elections.AddAsync(election, cancellationToken);
            return election;
        }

        public async Task<IReadOnlyList<Election>> GetAllElectionsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Elections
                .Include(e => e.ElectionStatus)
                .OrderByDescending(e => e.VotingEndDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Election?> GetElectionByIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            return await _dbContext.Elections
                .Include(e => e.ElectionStatus)
                .FirstOrDefaultAsync(e => e.Id == electionId, cancellationToken);
        }

        public void Update(Election election)
        {
            _dbContext.Elections.Update(election);
        }
 

        public async Task<Election?> GetLastUpcomingOrActiveElectionAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Elections
                .Include(e => e.ElectionStatus)
                .Where(e => e.ElectionStatusId != (int)ElectionStatusEnum.Completed)
                .OrderByDescending(e => e.VotingEndDate)
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<bool> IsActiveElectionExistsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Elections
               .Include(e => e.ElectionStatus)
               .AnyAsync(e => e.ElectionStatusId != (int)ElectionStatusEnum.Completed,cancellationToken);
        }

        
    }
}