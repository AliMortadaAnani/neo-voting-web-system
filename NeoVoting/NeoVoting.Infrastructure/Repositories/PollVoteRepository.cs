using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class PollVoteRepository : IPollVoteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PollVoteRepository> _logger;

        public PollVoteRepository(ApplicationDbContext context, ILogger<PollVoteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(PollVote vote)
        {
            _logger.LogInformation("Adding poll vote {VoteId} for poll {PollId}.", vote.Id, vote.PollId);
            _context.PollVotes.Add(vote);
        }

        public async Task<int> CountByPollIdAsync(int pollId)
        {
            _logger.LogInformation("Counting poll votes for poll {PollId}.", pollId);
            return await _context.PollVotes.CountAsync(pv => pv.PollId == pollId);
        }

        public async Task<PollVote?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching poll vote by id {VoteId}.", id);
            return await _context.PollVotes.FindAsync(id);
        }

        public async Task<List<PollVote>> GetPagedByPollIdAsync(int pollId, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching paged poll votes for poll {PollId} page {PageNumber} size {PageSize}.", pollId, pageNumber, pageSize);
            return await _context.PollVotes
                .AsNoTracking()
                .Where(pv => pv.PollId == pollId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<PollAnswerWithVotesDto>> GetResultsAsyncByPollId(int pollId)
        {
            _logger.LogInformation("Calculating poll results for poll {PollId}.", pollId);
            return await _context.PollAnswers
                .AsNoTracking()
                .Where(pa => pa.PollId == pollId)
                .OrderByDescending(pa => pa.PollVotes.Count)
                .ThenBy(cp => Guid.NewGuid())
                .Select(pa => new PollAnswerWithVotesDto
                {
                    pollAnswer = pa,
                    TotalVotes = pa.PollVotes.Count
                })
                .ToListAsync();
        }

        public async  Task<bool> IsPollAnswerExistByIdInPoll(int pollAnswerId, int pollId)
        {
            _logger.LogInformation("Validating poll answer {PollAnswerId} belongs to poll {PollId}.", pollAnswerId, pollId);
            return await _context.PollAnswers.AnyAsync(pa => pa.Id == pollAnswerId && pa.PollId == pollId);
        }
    }
}