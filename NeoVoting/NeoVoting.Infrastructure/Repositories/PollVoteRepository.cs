using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.EF_DTOs;
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

        public async Task<List<PollVote>> GetPagedByPollIdAsync(int pollId, int pageNumber, int pageSize)
        {
            return await _context.PollVotes
                .AsNoTracking()
                .Where(pv => pv.PollId == pollId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<PollAnswerWithVotesDto>> GetResultsAsyncByPollId(int pollId)
        {
            return await _context.PollAnswers
                .AsNoTracking()
                .Where(pa => pa.PollId == pollId)
                .OrderByDescending(pa => pa.PollVotes.Count)
                .ThenBy(cp => Guid.NewGuid()) // Randomize order for answers with the same vote count
                .Select(pa => new PollAnswerWithVotesDto
                {
                    pollAnswer = pa,
                    TotalVotes = pa.PollVotes.Count // EF Core handles this as a LEFT JOIN with a COUNT
                })
                .ToListAsync();
        }
    }
}