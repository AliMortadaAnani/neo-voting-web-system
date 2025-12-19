using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

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
            return await _dbContext.Governorates
                .OrderBy(g => g.Id)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}