using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionAndPollsStatisticsRepository : IElectionAndPollsStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public ElectionAndPollsStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ElectionAndPollStatistics?> GetByElectionIdAsync(int electionId)
        {
            return await _context.ElectionRegisteredVotersPerGovernorates
                .FirstOrDefaultAsync(eps => eps.ElectionId == electionId);
        }

        public async Task<ElectionAndPollStatistics?> GetByPollIdAsync(int pollId)
        {
            return await _context.ElectionRegisteredVotersPerGovernorates
                .FirstOrDefaultAsync(eps => eps.PollId == pollId);
        }

        public async Task<ElectionAndPollStatistics?> GetByElectionNameAsync(string electionName)
        {
            return await _context.ElectionRegisteredVotersPerGovernorates
                .Include(eps => eps.Election)
                .FirstOrDefaultAsync(eps => eps.Election != null && eps.Election.Name == electionName);
        }

        public async Task<ElectionAndPollStatistics?> GetByPollNameAsync(string pollName)
        {
            return await _context.ElectionRegisteredVotersPerGovernorates
                .Include(eps => eps.Poll)
                .FirstOrDefaultAsync(eps => eps.Poll != null && eps.Poll.Name == pollName);
        }

        public void Add(ElectionAndPollStatistics electionAndPollStatistics)
        {
            _context.ElectionRegisteredVotersPerGovernorates.Add(electionAndPollStatistics);
        }
    }
}