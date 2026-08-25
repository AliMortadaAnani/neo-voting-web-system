using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class CandidateProfileRepository : ICandidateProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public CandidateProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(CandidateProfile candidateProfile)
        {
            _context.CandidateProfiles.Add(candidateProfile);
        }

        public async Task<CandidateProfile?> GetByUserIdAndElectionIdAsync(int userId, int electionId)
        {
            return await _context.CandidateProfiles
                .Include(cp => cp.Candidate)
                .FirstOrDefaultAsync(cp => cp.Candidate.UserId == userId && cp.ElectionId == electionId);
        }

        public async Task<bool> IsCandidateProfileExistsByUserIdAndElectionIdAsync(int userId, int electionId)
        {
            return await _context.CandidateProfiles
                .AnyAsync(cp => cp.Candidate.UserId == userId && cp.ElectionId == electionId);
        }

        public async Task<List<CandidateProfile>> GetPagedByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate, int pageNumber, int pageSize)
        {
            return await _context.CandidateProfiles
                .Include(cp => cp.Candidate)
                .Where(cp => cp.ElectionId == electionId && cp.Candidate.Governorate == governorate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.CandidateProfiles
                .CountAsync(cp => cp.ElectionId == electionId && cp.Candidate.Governorate == governorate);
        }

        public async Task<int> CountByElectionIdAsync(int electionId)
        {
            return await _context.CandidateProfiles.CountAsync(cp => cp.ElectionId == electionId);
        }

        public async Task<int> CountByElectionIdAndGenderAsync(int electionId, char gender)
        {
            return await _context.CandidateProfiles
                .CountAsync(cp => cp.ElectionId == electionId && cp.Candidate.Gender == gender);
        }

        public async Task<int> CountsByElectionIdAndAgeRangeAsync(int electionId, int minAge, int maxAge)
        {
            var maxDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-minAge));
            var minDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-(maxAge + 1)).AddDays(1));

            return await _context.CandidateProfiles
                .CountAsync(cp => cp.ElectionId == electionId && 
                                  cp.Candidate.DateOfBirth >= minDate && 
                                  cp.Candidate.DateOfBirth <= maxDate);
        }

        public async Task<int> CountByElectionIdAndGovernorateAndGenderAsync(int electionId, GovernorateIdEnum governorate, char gender)
        {
            return await _context.CandidateProfiles
                .CountAsync(cp => cp.ElectionId == electionId && 
                                  cp.Candidate.Governorate == governorate && 
                                  cp.Candidate.Gender == gender);
        }

        public async Task<int> CountByElectionIdAndGovernorateAndAgeRangeAsync(int electionId, GovernorateIdEnum governorate, int minAge, int maxAge)
        {
            var maxDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-minAge));
            var minDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-(maxAge + 1)).AddDays(1));

            return await _context.CandidateProfiles
                .CountAsync(cp => cp.ElectionId == electionId && 
                                  cp.Candidate.Governorate == governorate && 
                                  cp.Candidate.DateOfBirth >= minDate && 
                                  cp.Candidate.DateOfBirth <= maxDate);
        }
    }
}