using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.EF_DTOs;
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


        public async Task<List<Vote>> GetPagedByElectionIdAsync(
                                    int electionId,
                                    int pageNumber,
                                    int pageSize)
        {
            var query = _context.Votes
                .AsNoTracking()
                .Where(v => v.ElectionId == electionId);

            return await query
                .OrderByDescending(v => v.TimestampUTC)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByElectionIdAsync(
            int electionId)
        {
            var query = _context.Votes
                .Where(v => v.ElectionId == electionId);

            return await query.CountAsync();
        }

        public async Task<List<CandidateProfileWithVotesDto>> GetPagedCandidatesProfilesResultsAsync(int electionId, int pageNumber, int pageSize)
        {
            var query = _context.CandidateProfiles
               .AsNoTracking()
               .Where(cp => cp.ElectionId == electionId);

            return await query
                // 1. First, sort by vote count descending (highest votes come first)
                .OrderByDescending(cp => cp.Votes.Count)

                // 2. Then, if multiple candidates share the exact same vote count, randomize using Guid.
                .ThenBy(cp => Guid.NewGuid())

                .Select(cp => new CandidateProfileWithVotesDto
                {
                    CandidateProfile = cp,
                    TotalVotes = cp.Votes.Count
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


    }
}