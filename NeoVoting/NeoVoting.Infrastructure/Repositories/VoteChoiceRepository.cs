using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteChoiceRepository : IVoteChoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteChoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(VoteChoice voteChoice)
        {
            _context.VoteChoices.Add(voteChoice);
        }

        public async Task<bool> IsVoteChoiceExistsByVoteIdAndCandidateProfileIdAsync(Guid voteId, int candidateProfileId)
        {
            return await _context.VoteChoices
                .AnyAsync(vc => vc.VoteId == voteId && vc.CandidateProfileId == candidateProfileId);
        }

        public async Task<int> GetCountOfTotalVoteChoicesByCandidateProfileIdAsync(int candidateProfileId)
        {
            return await _context.VoteChoices
                .CountAsync(vc => vc.CandidateProfileId == candidateProfileId);
        }

        public async Task<List<CandidateProfile>> GetTop5CandidatesProfilesPerGovernorate(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.CandidateProfiles
                .AsNoTracking()
                .Where(cp => cp.ElectionId == electionId && cp.Candidate.Governorate == governorate)
                // 1. First, sort by vote count descending (highest votes come first)
                .OrderByDescending(cp => cp.VoteChoices.Count)

                // 2. Then, if multiple candidates share the exact same vote count (e.g., a tie at 0 or 5 votes),
                // randomize their positions using a Guid.
                .ThenBy(cp => Guid.NewGuid())

                .Take(5)
                .ToListAsync();
        }
    }
}