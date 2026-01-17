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

        public async Task<Election?> GetElectionByIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.Elections
                .Include(e => e.ElectionStatus)
                .FirstOrDefaultAsync(e => e.Id == ElectionId, cancellationToken);
        }

        public void Update(Election election)
        {
            _dbContext.Elections.Update(election);
        }


        // --- NEW STATS METHODS ---

        public async Task<Election?> GetLastCompletedElectionAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Elections
                .Include(e => e.ElectionStatus)
                .Where(e => e.ElectionStatusId == (int)ElectionStatusEnum.Completed)
                .OrderByDescending(e => e.VotingEndDate)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Election?> GetLastUpcomingOrActiveElectionAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Elections
                .Include(e => e.ElectionStatus)
                .Where(e => e.ElectionStatusId != (int)ElectionStatusEnum.Completed)
                .OrderByDescending(e => e.VotingEndDate)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int?> GetRegisteredVotersCountByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            // Returns the frozen count stored in the entity if completed, otherwise null
            var election = await _dbContext.Elections
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == electionId, cancellationToken);

            return election?.FinalNumberOfRegisteredVoters;
        }

        //Task<int> IElectionRepository.GetRegisteredVotersCountByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}
    }
}