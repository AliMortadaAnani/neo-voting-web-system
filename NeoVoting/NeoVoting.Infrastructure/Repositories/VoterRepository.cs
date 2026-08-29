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
        public async Task<Voter?> GetByVerificationHashAsync(string verificationHash)
        {
            return await _context.Voters
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.VerificationHash == verificationHash);
        }
        


        public async Task<int> CountAsync()
        {
            return await _context.Voters.CountAsync();
        }

        public async Task<int> CountByGovernorateAsync(GovernorateIdEnum governorate)
        {
            return await _context.Voters.CountAsync(v => v.Governorate == governorate);
        }

        public async Task<int> CountByGenderAsync(char gender)
        {
            return await _context.Voters.CountAsync(v => v.Gender == gender);
        }

        public async Task<int> CountsByAgeRangeAsync(int minAge, int maxAge)
        {
            var maxDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-minAge));
            var minDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-(maxAge + 1)).AddDays(1));

            return await _context.Voters
                .CountAsync(v => v.DateOfBirth >= minDate && v.DateOfBirth <= maxDate);
        }

        public async Task<int> CountByGovernorateAndGenderAsync(GovernorateIdEnum governorate, char gender)
        {
            return await _context.Voters
                .CountAsync(v => v.Governorate == governorate && v.Gender == gender);
        }

        public async Task<int> CountByGovernorateAndAgeRangeAsync(GovernorateIdEnum governorate, int minAge, int maxAge)
        {
            var maxDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-minAge));
            var minDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-(maxAge + 1)).AddDays(1));

            return await _context.Voters
                .CountAsync(v => v.Governorate == governorate &&
                                 v.DateOfBirth >= minDate &&
                                 v.DateOfBirth <= maxDate);
        }


        

        

    }
}