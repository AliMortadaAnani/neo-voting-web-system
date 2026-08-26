using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class PollVoteRepository : IPollVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public PollVoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(PollVote vote)
        {
            _context.PollVotes.Add(vote);
        }

        public async Task<int> CountByPollIdAsync(int pollId)
        {
            return await _context.PollVotes.CountAsync(pv => pv.PollId == pollId);
        }

        public async Task<PollVote?> GetByPollVoteIdAsync(Guid pollVoteId)
        {
            return await _context.PollVotes
                .FindAsync(pollVoteId);
        }

        public async Task<List<PollVote>> GetPagedByPollIdAsync(int pollId, int pageNumber, int pageSize)
        {
            return await _context.PollVotes
                .Where(pv => pv.PollId == pollId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<PollAnswer>> GetResultsAsyncByPollId(int pollId)
        {
            // Assuming this means get all answers with their votes included to calculate results
            return await _context.PollAnswers
                .Include(pa => pa.PollVotes)
                .Where(pa => pa.PollId == pollId)
                .ToListAsync();
        }

        public async Task<PollAnswer?> GetWinnerAnswerByPollIdAsync(int pollId)
        {
            return await _context.PollAnswers
                .Where(pa => pa.PollId == pollId)
                .OrderByDescending(pa => pa.PollVotes.Count)
                .FirstOrDefaultAsync();
        }
    }
}