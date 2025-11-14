using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;
using System.Threading;

namespace NeoVoting.Infrastructure.Repositories
{
    public class GovernorateRepository : IGovernorateRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GovernorateRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IReadOnlyList<Governorate>> GetAllGovernoratesAsync(CancellationToken cancellationToken)
        {
          return await _dbContext.Governorates.ToListAsync(cancellationToken);
        }
    }
}