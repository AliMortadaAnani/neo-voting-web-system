using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class CandidateProfileRepository : ICandidateProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CandidateProfileRepository> _logger;

        public CandidateProfileRepository(ApplicationDbContext context, ILogger<CandidateProfileRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsCandidateProfileExistsByCandidateIdAndElectionIdAsync(int candidateId, int electionId)
        {
            _logger.LogInformation("Checking whether candidate {CandidateId} is already nominated for election {ElectionId}.", candidateId, electionId);
            return await _context.CandidateProfiles
                .AnyAsync(cp => cp.CandidateId == candidateId && cp.ElectionId == electionId);
        }

        public void Add(CandidateProfile candidateProfile)
        {
            _logger.LogInformation("Adding candidate profile {CandidateProfileId} for candidate {CandidateId} in election {ElectionId}.", candidateProfile.Id, candidateProfile.CandidateId, candidateProfile.ElectionId);
            _context.CandidateProfiles.Add(candidateProfile);
        }

        public async Task<CandidateProfile?> GetByCandidateIdAndElectionIdAsync(int candidateId, int electionId)
        {
            _logger.LogInformation("Fetching candidate profile for candidate {CandidateId} in election {ElectionId}.", candidateId, electionId);
            return await _context.CandidateProfiles
                .Include(cp => cp.Candidate)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(cp => cp.CandidateId == candidateId && cp.ElectionId == electionId);
        }

        public async Task<List<CandidateProfile>> GetPagedByElectionIdAsync(
      int electionId,
      int pageNumber,
      int pageSize)
        {
            _logger.LogInformation("Fetching paged candidate profiles for election {ElectionId} page {PageNumber} size {PageSize}.", electionId, pageNumber, pageSize);
            var query = _context.CandidateProfiles
                .Include(cp => cp.Candidate)
                .AsNoTracking()
                .Where(cp => cp.ElectionId == electionId);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByElectionIdAsync(
            int electionId)
        {
            _logger.LogInformation("Counting candidate profiles for election {ElectionId}.", electionId);
            var query = _context.CandidateProfiles
                .Where(cp => cp.ElectionId == electionId);

            return await query.CountAsync();
        }

        public async Task<bool> IsCandidateProfileExistsInElectionAsync(int candidateProfileId, int electionId)
        {
            _logger.LogInformation("Validating candidate profile {CandidateProfileId} belongs to election {ElectionId}.", candidateProfileId, electionId);
            return await _context.CandidateProfiles
                .AnyAsync(cp => cp.Id == candidateProfileId && cp.ElectionId == electionId);
        }
    }
}