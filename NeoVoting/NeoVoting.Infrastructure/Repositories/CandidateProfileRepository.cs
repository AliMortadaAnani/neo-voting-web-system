using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class CandidateProfileRepository : ICandidateProfileRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CandidateProfileRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CandidateProfile> AddCandidateProfileAsync(CandidateProfile candidateProfile, CancellationToken cancellationToken)
        {
            await _dbContext.CandidateProfiles.AddAsync(candidateProfile, cancellationToken);
            return candidateProfile;
        }

        public async Task<List<CandidateProfile>> GetAllCandidatesProfilesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .ToListAsync(cancellationToken);
        }

        public async Task<CandidateProfile?> GetCandidateProfileByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election).
                FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public void Update(CandidateProfile candidateProfile)
        {
            _dbContext.CandidateProfiles.Update(candidateProfile);
        }

    }
}