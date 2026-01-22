using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionStatisticsRepository (ApplicationDbContext dbContext) : IElectionStatisticsRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task<ElectionStatistics> AddAsync(ElectionStatistics entity, CancellationToken cancellationToken)
        {
           await _dbContext.ElectionRegisteredVotersPerGovernorates.AddAsync(entity, cancellationToken);
            return entity;
        }
        // governorateId is nullable because it can represent the total registered voters across all governorates when null
        public async Task<ElectionStatistics?> GetByElectionIdAndGovernorateIdAsync(Guid electionId, int? governorateId, CancellationToken cancellationToken)
        {
            return await _dbContext.ElectionRegisteredVotersPerGovernorates
                .FirstOrDefaultAsync(erpg => erpg.ElectionId == electionId && erpg.GovernorateId == governorateId, cancellationToken);
        }
    }
}
