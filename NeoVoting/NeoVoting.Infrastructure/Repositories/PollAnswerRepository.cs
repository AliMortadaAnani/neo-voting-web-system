using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class PollAnswerRepository : IPollAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public PollAnswerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(PollAnswer answer)
        {
            _context.PollAnswers.Add(answer);
        }

        public async Task<List<PollAnswer>> GetAllAnswersByPollIdAsync(int pollId)
        {
            return await _context.PollAnswers
                .AsNoTracking()
                .Include(x => x.Poll)
                .Where(pa => pa.PollId == pollId)
                .ToListAsync();
        }

        public async Task<bool> IsPollAnswerExistsAsync(int pollId, string answerText)
        {
            return await _context.PollAnswers.AnyAsync(pa => pa.PollId == pollId && pa.Answer == answerText);
        }
    }
}