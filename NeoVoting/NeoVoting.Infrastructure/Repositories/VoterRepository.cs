using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoterRepository : IVoterRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VoterRepository> _logger;

        public VoterRepository(ApplicationDbContext context, ILogger<VoterRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsVoterExistByVerificationHashAsync(string verificationHash)
        {
            _logger.LogInformation("Checking whether a voter exists for verification hash {VerificationHash}.", verificationHash);
            return await _context.Voters.AnyAsync(v => v.VerificationHash == verificationHash);
        }

        public void Add(Voter voter)
        {
            _logger.LogInformation("Adding voter {VoterId} for user {UserId}.", voter.Id, voter.UserId);
            _context.Voters.Add(voter);
        }

        public async Task<Voter?> GetByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching voter record for user {UserId}.", userId);
            return await _context.Voters
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }
    }
}