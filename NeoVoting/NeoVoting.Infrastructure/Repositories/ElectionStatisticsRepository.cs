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

        public async Task<ElectionStatistics?> GetByElectionIdAsync(int electionId)
        {
            return await _context.Election_Statistics
                .Include(eps => eps.Election)
                .FirstOrDefaultAsync(eps => eps.ElectionId == electionId);
        }

        public async Task<ElectionStatistics?> GetByElectionNameAsync(string electionName)
        {
            return await _context.Election_Statistics
                .Include(eps => eps.Election)
                .FirstOrDefaultAsync(eps => eps.Election.Name == electionName);
        }

        public void Add(ElectionStatistics electionStatistics)
        {
            _context.Election_Statistics.Add(electionStatistics);
        }

        public async Task<ElectionStatistics?> GetByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.Election_Statistics
                .Include(eps => eps.Election)
                .FirstOrDefaultAsync(eps => eps.ElectionId == electionId && eps.Governorate == governorate);
        }

        public async Task<ElectionStatistics?> GetByElectionNameAndGovernorateAsync(string electionName, GovernorateIdEnum governorate)
        {
            return await _context.Election_Statistics
                .Include(eps => eps.Election)
                .FirstOrDefaultAsync(eps => eps.Election.Name == electionName && eps.Governorate == governorate);
        }
    }
}