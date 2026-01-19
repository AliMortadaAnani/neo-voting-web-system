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

        public async Task<IReadOnlyList<CandidateProfile>> GetAllCandidatesProfilesByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .Where(c => c.ElectionId == electionId)
                .OrderBy(c => c.User.UserName)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<CandidateProfile?> GetCandidateProfileByUserIdAndElectionIdAsync(Guid userId, Guid electionId, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ElectionId == electionId, cancellationToken);
        }

        public async Task<IReadOnlyList<CandidateProfile>> GetPagedCandidatesProfilesByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .Where(c => c.ElectionId == electionId)
                .OrderBy(c => c.User.UserName)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountOfTotalCandidatesProfilesByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles.CountAsync(c => c.ElectionId == electionId, cancellationToken);
        }

        public void Update(CandidateProfile candidateProfile)
        {
            _dbContext.CandidateProfiles.Update(candidateProfile);
        }

        

        public async Task<IReadOnlyList<CandidateProfile>> GetPagedCandidatesProfilesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
                .Include(c => c.User)
                .Include(c => c.Election)
                .Where(c => c.ElectionId == electionId && c.User.GovernorateId == governorateId)
                .OrderBy(c => c.User.UserName)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountOfTotalCandidatesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken)
        {
            // CandidateProfile does not have GovernorateId directly, it's on the User.
            return await _dbContext.CandidateProfiles
                //.Include(c => c.User)
                .Where(c => c.ElectionId == electionId && c.User.GovernorateId == governorateId)
                .CountAsync(cancellationToken);
        }

        public async Task<bool> IsCandidateProfileExistsByUserIdAndElectionIdAsync(Guid userId, Guid electionId, CancellationToken cancellationToken)
        {
            return await _dbContext.CandidateProfiles
        .AnyAsync(cp => cp.UserId == userId && cp.ElectionId == electionId, cancellationToken);
        }
    }
}