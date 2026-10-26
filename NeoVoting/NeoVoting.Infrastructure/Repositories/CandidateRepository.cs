using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CandidateRepository> _logger;

        public CandidateRepository(ApplicationDbContext context, ILogger<CandidateRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsCandidateExistByVerificationHashAsync(string verificationHash)
        {
            _logger.LogInformation("Checking whether a candidate exists for verification hash {VerificationHash}.", verificationHash);
            return await _context.Candidates.AnyAsync(c => c.VerificationHash == verificationHash);
        }

        public void Add(Candidate candidate)
        {
            _logger.LogInformation("Adding candidate {CandidateId} for user {UserId}.", candidate.Id, candidate.UserId);
            _context.Candidates.Add(candidate);
        }

        public async Task<Candidate?> GetByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching candidate record for user {UserId}.", userId);
            return await _context.Candidates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}