using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VoteRepository> _logger;

        public VoteRepository(ApplicationDbContext context, ILogger<VoteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Vote vote)
        {
            _logger.LogInformation("Adding election vote {VoteId} for election {ElectionId} and candidate profile {CandidateProfileId}.", vote.Id, vote.ElectionId, vote.CandidateProfileId);
            _context.Votes.Add(vote);
        }

        public async Task<List<Vote>> GetPagedByElectionIdAsync(
                                    int electionId,
                                    int pageNumber,
                                    int pageSize)
        {
            _logger.LogInformation("Fetching paged election votes for election {ElectionId} page {PageNumber} size {PageSize}.", electionId, pageNumber, pageSize);
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
            _logger.LogInformation("Counting election votes for election {ElectionId}.", electionId);
            var query = _context.Votes
                .Where(v => v.ElectionId == electionId);

            return await query.CountAsync();
        }

        public async Task<List<CandidateProfileWithVotesDto>> GetPagedCandidatesProfilesResultsAsync(int electionId, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching paged candidate vote results for election {ElectionId} page {PageNumber} size {PageSize}.", electionId, pageNumber, pageSize);
            var query = _context.CandidateProfiles
                .Include(cp => cp.Candidate)
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

        public async Task<Vote?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching election vote by id {VoteId}.", id);
            return await _context.Votes.FindAsync(id);
        }
    }
}