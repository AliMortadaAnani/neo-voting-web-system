using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _context;

        public CandidateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCandidateExistByVerificationHashAsync(string verificationHash)
        {
            return await _context.Candidates.AnyAsync(c => c.VerificationHash == verificationHash);
        }

        public async Task<Candidate?> GetByVerificationHashAsync(string verificationHash)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.VerificationHash == verificationHash);
        }

        public async Task<Candidate?> GetByUserIdAsync(int userId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public void Add(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
        }
    }
}