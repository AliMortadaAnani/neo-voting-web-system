using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CandidateRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddCandidate(Candidate candidate)
        {
            _dbContext.Candidates.Add(candidate);
        }

        public void Delete(Candidate candidate)
        {
            _dbContext.Candidates.Remove(candidate);
        }

        public Task<Candidate?> GetCandidateByNationalIdAsync(string nationalId)
        {
            var candidate = _dbContext.Candidates
                .Include(c => c.Citizen)
                .SingleOrDefaultAsync(c => c.Citizen.NationalId == nationalId);
            return candidate;
        }

        public Task<Candidate?> GetCandidateByHashedDataAsync(string hashedData)
        {
            var candidate = _dbContext.Candidates
                .Include(c => c.Citizen)
                .SingleOrDefaultAsync(c => c.HashedData == hashedData);
            return candidate;
        }

        public Task<List<Candidate>> GetCandidatesPagedAsync(int pageNumber, int pageSize)
        {
            var candidates = _dbContext.Candidates
                 .Include(v => v.Citizen)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
            return candidates;
        }

        public Task<int> GetCandidatesTotalCountAsync()
        {
            return _dbContext.Candidates.CountAsync();
        }
    }
}