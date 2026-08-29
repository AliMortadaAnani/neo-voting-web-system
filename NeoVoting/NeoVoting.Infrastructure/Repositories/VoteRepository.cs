using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Vote vote)
        {
            _context.Votes.Add(vote);
        }

        public async Task<bool> IsVoteChoicesForVoteEqualFive(Vote vote)
        {
            // Relies on VoteChoices being loaded or checking against the DB
            return await _context.VoteChoices.CountAsync(vc => vc.VoteId == vote.Id) == 5;
        }

        public async Task<Vote?> GetByVoteId(Guid voteId)
        {
            return await _context.Votes

                .FindAsync(voteId);
        }

        public async Task<List<Vote>> GetPagedByElectionIdAsync(int electionId, int pageNumber, int pageSize)
        {
            return await _context.Votes
                .AsNoTracking()
                .Where(v => v.ElectionId == electionId)
                .OrderByDescending(v => v.TimestampUTC) // Assuming you want to order by timestamp
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}