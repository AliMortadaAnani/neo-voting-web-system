using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
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

        public async Task<List<Vote>> GetPagedByElectionIdAsync(
    int electionId,
    GovernorateIdEnum? governorate,
    int pageNumber,
    int pageSize)
        {
            var query = _context.Votes
                .AsNoTracking()
                .Where(v => v.ElectionId == electionId);

            // If governorate is provided, filter by it. If null, it gets all votes for the election.
            if (governorate.HasValue)
            {
                query = query.Where(v => v.Governorate == governorate.Value);
            }

            return await query
                .OrderByDescending(v => v.TimestampUTC)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByElectionIdAsync(
            int electionId,
            GovernorateIdEnum? governorate)
        {
            var query = _context.Votes
                .Where(v => v.ElectionId == electionId);

            if (governorate.HasValue)
            {
                query = query.Where(v => v.Governorate == governorate.Value);
            }

            return await query.CountAsync();
        }

    }
}