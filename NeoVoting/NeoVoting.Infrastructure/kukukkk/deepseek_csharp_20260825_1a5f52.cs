using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class PollRepository : IPollRepository
    {
        private readonly ApplicationDbContext _context;

        public PollRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Poll poll)
        {
            _context.Polls.Add(poll);
        }

        public async Task<bool> IsActivePollExistsAsync()
        {
            return await _context.Polls.AnyAsync(p => p.Status == StatusEnum.Voting);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Polls.CountAsync();
        }

        public async Task<List<Poll>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Polls
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Poll?> GetByIdAsync(int pollId)
        {
            return await _context.Polls.FindAsync(pollId);
        }

        public async Task<Poll?> GetByNameAsync(string pollName)
        {
            return await _context.Polls.FirstOrDefaultAsync(p => p.Name == pollName);
        }

        public async Task<Poll?> GetActivePollAsync()
        {
            return await _context.Polls.FirstOrDefaultAsync(p => p.Status == StatusEnum.Voting);
        }
    }
}