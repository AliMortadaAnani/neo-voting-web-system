using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionStatisticsRepository : IElectionStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public ElectionStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(ElectionStatistics electionStatistics)
        {
            _context.Election_Statistics.Add(electionStatistics);
        }

        // Single unified method supporting optional governorate lookup
        public async Task<ElectionStatistics?> GetByElectionIdAsync(int electionId, GovernorateIdEnum? governorate)
        {
            var query = _context.Election_Statistics
                .Include(eps => eps.Election)
                .Where(eps => eps.ElectionId == electionId);

            if (!governorate.HasValue)
            {
                // If governorate is null, return the first record (Global/Overall)
                return await query.FirstOrDefaultAsync();
            }

            // If governorate has a value, return the record for that specific governorate
            return await query.FirstOrDefaultAsync(eps => eps.Governorate == governorate.Value);
        }
    }
}