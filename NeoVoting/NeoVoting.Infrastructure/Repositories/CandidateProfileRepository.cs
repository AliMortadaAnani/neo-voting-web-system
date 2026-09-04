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

        public async Task<bool> IsCandidateProfileExistsByCandidateIdAndElectionIdAsync(int candidateId, int electionId)
        {
            return await _context.CandidateProfiles
                .AnyAsync(cp => cp.CandidateId == candidateId && cp.ElectionId == electionId);
        }

        public void Add(CandidateProfile candidateProfile)
        {
            _context.CandidateProfiles.Add(candidateProfile);
        }

        public async Task<CandidateProfile?> GetByCandidateIdAndElectionIdAsync(int candidateId, int electionId)
        {
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
            var query = _context.CandidateProfiles
                .Where(cp => cp.ElectionId == electionId);

            return await query.CountAsync();
        }

     
    }
}