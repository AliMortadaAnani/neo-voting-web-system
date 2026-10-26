using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class PollRepository : IPollRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PollRepository> _logger;

        public PollRepository(ApplicationDbContext context, ILogger<PollRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsActivePollExistsAsync()
        {
            _logger.LogInformation("Checking whether any active poll exists.");
            return await _context.Polls.AnyAsync(p => p.Status != StatusEnum.Completed);
        }

        public async Task<bool> IsPollNameExistsAsync(string pollName)
        {
            _logger.LogInformation("Checking whether poll name {PollName} already exists.", pollName);
            return await _context.Polls.AnyAsync(p => p.Name == pollName);
        }

        public void Add(Poll poll)
        {
            _logger.LogInformation("Adding poll {PollId} named {PollName}.", poll.Id, poll.Name);
            _context.Polls.Add(poll);
        }

        public async Task<Poll?> GetByIdAsync(int pollId)
        {
            _logger.LogInformation("Fetching poll {PollId}.", pollId);
            return await _context.Polls
                .Include(p => p.PollAnswers)
                .FirstOrDefaultAsync(p => p.Id == pollId);
        }

        public async Task<int> CountAsync()
        {
            _logger.LogInformation("Counting polls.");
            return await _context.Polls.CountAsync();
        }

        public async Task<List<Poll>> GetPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching paged polls page {PageNumber} size {PageSize}.", pageNumber, pageSize);
            return await _context.Polls
                .AsNoTracking()
                .Include(p => p.PollAnswers)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Poll?> GetActivePollAsync()
        {
            _logger.LogInformation("Fetching active poll.");
            return await _context.Polls
                .Include(p => p.PollAnswers)
                .SingleOrDefaultAsync(p => p.Status != StatusEnum.Completed);
        }
    }
}