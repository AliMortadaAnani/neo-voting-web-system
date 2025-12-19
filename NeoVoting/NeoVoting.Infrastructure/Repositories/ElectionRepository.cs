using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionRepository : IElectionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ElectionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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
    }
}