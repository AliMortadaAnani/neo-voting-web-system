using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.EF_DTOs;
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

        // to specify winners and add them to ElectionWinners table, we need to get the top 5 candidates profiles per governorate based on their vote count => insert them into ElectionWinners table
        public async Task<List<CandidateResultResponseEF_DTO>> GetTop5CandidatesProfilesPerGovernorateAsync(int electionId, GovernorateIdEnum governorate)
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
                .Select(cp => new CandidateResultResponseEF_DTO
                {
                    CandidateProfileId = cp.Id,
                    FirstName = cp.Candidate.FirstName,
                    LastName = cp.Candidate.LastName,
                    ProfilePhotoFilename = cp.ProfilePhotoFilename ?? string.Empty,
                    Governorate = cp.Candidate.Governorate,
                    VoteCount = cp.VoteChoices.Count // Evaluated efficiently by EF Core as a SQL COUNT aggregate
                })
                .ToListAsync();
        }


        public async Task<List<CandidateResultResponseEF_DTO>> GetPagedCandidatesProfilesResultsAsync(
    int electionId,
    GovernorateIdEnum? governorate,
    int pageNumber,
    int pageSize)
        {
            var query = _context.CandidateProfiles
                .AsNoTracking()
                .Where(cp => cp.ElectionId == electionId);

            // Apply governorate filter conditionally if provided
            if (governorate.HasValue)
            {
                query = query.Where(cp => cp.Candidate.Governorate == governorate.Value);
            }

            return await query
                // 1. First, sort by vote count descending (highest votes come first)
                .OrderByDescending(cp => cp.VoteChoices.Count)

                // 2. Then, if multiple candidates share the exact same vote count, randomize using Guid.
                .ThenBy(cp => Guid.NewGuid())

                .Select(cp => new CandidateResultResponseEF_DTO
                {
                    CandidateProfileId = cp.Id,
                    FirstName = cp.Candidate.FirstName,
                    LastName = cp.Candidate.LastName,
                    ProfilePhotoFilename = cp.ProfilePhotoFilename ?? string.Empty,
                    Governorate = cp.Candidate.Governorate,
                    VoteCount = cp.VoteChoices.Count
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountCandidatesProfilesResultsAsync(
            int electionId,
            GovernorateIdEnum? governorate)
        {
            var query = _context.CandidateProfiles
                .Where(cp => cp.ElectionId == electionId);

            if (governorate.HasValue)
            {
                query = query.Where(cp => cp.Candidate.Governorate == governorate.Value);
            }

            return await query.CountAsync();
        }
    }
}