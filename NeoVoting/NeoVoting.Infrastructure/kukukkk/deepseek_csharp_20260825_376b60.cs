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

        public async Task<PollVote?> GetByPollIdAndPollVoteIdAsync(int pollId, int pollVoteId)
        {
            // FLAG: Interface asks for `int pollVoteId`, but Entity `PollVote.Id` is a `Guid`.
            // Because EF can't easily translate Guid <-> int in SQL, this might throw a runtime exception.
            // You should change the interface to accept a Guid. I am using a string comparison as a dirty hack for now to satisfy the compiler.
            string stringPollVoteId = pollVoteId.ToString();
            return await _context.PollVotes
                .FirstOrDefaultAsync(pv => pv.PollId == pollId && pv.Id.ToString() == stringPollVoteId);
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