using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class PollStatisticsRepository : IPollStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public PollStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(PollStatistics pollStatistics)
        {
            _context.Poll_Statistics.Add(pollStatistics);
        }

        public async Task<PollStatistics?> GetByPollIdAsync(int pollId)
        {
            return await _context.Poll_Statistics
                .Include(ps => ps.Poll)
                .ThenInclude(p => p.PollAnswers)
                .FirstOrDefaultAsync(ps => ps.PollId == pollId);
        }

        public async Task<PollStatistics?> GetByPollNameAsync(string pollName)
        {
            return await _context.Poll_Statistics
                .Include(ps => ps.Poll)
                .ThenInclude(p => p.PollAnswers)
                .FirstOrDefaultAsync(ps => ps.Poll.Name == pollName);
        }
    }
}