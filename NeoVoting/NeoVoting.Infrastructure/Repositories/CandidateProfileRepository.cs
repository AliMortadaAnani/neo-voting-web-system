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

        public async Task<CandidateProfile?> GetByCandidateIdAndElectionIdAsync(int candidateId, int electionId)
        {
            return await _context.CandidateProfiles
                .Include(cp => cp.Candidate)
                .FirstOrDefaultAsync(cp => cp.CandidateId == candidateId && cp.ElectionId == electionId);
        }

        public async Task<bool> IsCandidateProfileExistsByCandidateIdAndElectionIdAsync(int candidateId, int electionId)
        {
            return await _context.CandidateProfiles
                .AnyAsync(cp => cp.CandidateId == candidateId && cp.ElectionId == electionId);
        }

        public async Task<List<CandidateProfile>> GetPagedByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate, int pageNumber, int pageSize)
        {
            return await _context.CandidateProfiles
                .Include(cp => cp.Candidate)
                .AsNoTracking()
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
            // Use DateTime.Today to align with local calendar dates rather than UTC shifts
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxDate = today.AddYears(-minAge);
            var minDate = today.AddYears(-(maxAge + 1)).AddDays(1);

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
            // Use DateTime.Today to align with local calendar dates rather than UTC shifts
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxDate = today.AddYears(-minAge);
            var minDate = today.AddYears(-(maxAge + 1)).AddDays(1);

            return await _context.CandidateProfiles
                .CountAsync(cp => cp.ElectionId == electionId &&
                                  cp.Candidate.Governorate == governorate &&
                                  cp.Candidate.DateOfBirth >= minDate &&
                                  cp.Candidate.DateOfBirth <= maxDate);
        }
    }
}