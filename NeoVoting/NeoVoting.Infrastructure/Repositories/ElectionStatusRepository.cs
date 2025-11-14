using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionStatusRepository : IElectionStatusRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ElectionStatusRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IReadOnlyList<ElectionStatus>> GetAllElectionStatusesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.ElectionStatuses.ToListAsync(cancellationToken);
        }
    }
}