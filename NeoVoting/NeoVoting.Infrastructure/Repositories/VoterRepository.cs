using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoterRepository : IVoterRepository
    {
        private readonly ApplicationDbContext _context;

        public VoterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsVoterExistByVerificationHashAsync(string verificationHash)
        {
            return await _context.Voters.AnyAsync(v => v.VerificationHash == verificationHash);
        }

        public void Add(Voter voter)
        {
            _context.Voters.Add(voter);
        }


        public async Task<Voter?> GetByUserIdAsync(int userId)
        {
            return await _context.Voters
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }


    }
}