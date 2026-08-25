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

        public async Task<bool> IsVoteChoiceExistsByVoteIdAndCandidateProfileIdAsync(int voteId, int candidateProfileId)
        {
            // FLAG: Interface uses `int voteId`, but `VoteChoice.VoteId` is a `Guid`.
            // Modifying the interface to accept a Guid is highly recommended. Using string comparison as a workaround.
            string stringVoteId = voteId.ToString();
            return await _context.VoteChoices
                .AnyAsync(vc => vc.VoteId.ToString() == stringVoteId && vc.CandidateProfileId == candidateProfileId);
        }

        public async Task<int> GetCountOfTotalVoteChoicesByCandidateProfileIdAsync(int candidateProfileId)
        {
            return await _context.VoteChoices
                .CountAsync(vc => vc.CandidateProfileId == candidateProfileId);
        }

        public async Task<List<CandidateProfile>> GetTop5CandidatesProfilesPerGovernorate(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.CandidateProfiles
                .Where(cp => cp.ElectionId == electionId && cp.Candidate.Governorate == governorate)
                .OrderByDescending(cp => cp.VoteChoices.Count)
                .Take(5)
                .ToListAsync();
        }
    }
}