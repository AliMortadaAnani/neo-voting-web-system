using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class CandidateProfileRepository(ApplicationDbContext dbContext) : ICandidateProfileRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CandidateProfile> AddCandidateProfileAsync(CandidateProfile candidateProfile, CancellationToken cancellationToken)
        {
            await _dbContext.CandidateProfiles.AddAsync(candidateProfile, cancellationToken);
            return candidateProfile;
        }

        public async Task<IReadOnlyList<CandidateProfile>> GetAllCandidatesProfilesByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .Where(c => c.ElectionId == ElectionId)
                .OrderBy(c => c.User.UserName)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<CandidateProfile?> GetCandidateProfileByUserIdAndElectionIdAsync(Guid UserId, Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .FirstOrDefaultAsync(c => c.UserId == UserId && c.ElectionId == ElectionId, cancellationToken);
        }

        public async Task<IReadOnlyList<CandidateProfile>> GetPagedCandidatesProfilesByElectionIdAsync(Guid ElectionId, int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .Where(c => c.ElectionId == ElectionId)
                .OrderBy(c => c.User.UserName)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalCandidatesProfilesCountByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles.CountAsync(c => c.ElectionId == ElectionId, cancellationToken);
        }

        public void Update(CandidateProfile candidateProfile)
        {
            _dbContext.CandidateProfiles.Update(candidateProfile);
        }

        // --- NEW STATS METHODS ---

        public async Task<int> GetCandidatesCountByElectionAndGovernorateAsync(Guid electionId, int governorateId, CancellationToken cancellationToken)
        {
            // CandidateProfile does not have GovernorateId directly, it's on the User.
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Where(c => c.ElectionId == electionId && c.User.GovernorateId == governorateId)
                .CountAsync(cancellationToken);
        }

        public Task<IReadOnlyList<CandidateProfile>> GetPagedCandidatesProfilesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalCandidatesCountByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCandidateProfileExistsByUserIdAndElectionIdAsync(Guid userId, Guid electionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}