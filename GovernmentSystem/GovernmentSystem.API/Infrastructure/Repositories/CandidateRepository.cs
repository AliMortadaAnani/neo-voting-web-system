using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CandidateRepository> _logger;

        public CandidateRepository(ApplicationDbContext dbContext, ILogger<CandidateRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public void Add(Candidate candidate)
        {
            _logger.LogInformation("CandidateRepository: Adding new candidate");
            _dbContext.Candidates.Add(candidate);
        }

        public void Delete(Candidate candidate)
        {
            _logger.LogInformation("CandidateRepository: Deleting candidate");
            _dbContext.Candidates.Remove(candidate);
        }

        public Task<Candidate?> GetCandidateByNationalIdAsync(string nationalId)
        {
            _logger.LogInformation("CandidateRepository: Fetching candidate by NationalId");
            var candidate = _dbContext.Candidates
                .Include(c => c.Citizen)
                .SingleOrDefaultAsync(c => c.Citizen.NationalId == nationalId);
            return candidate;
        }

        public Task<Candidate?> GetCandidateByHashedDataAsync(string hashedData)
        {
            _logger.LogInformation("CandidateRepository: Fetching candidate by HashedData");
            var candidate = _dbContext.Candidates
                .Include(c => c.Citizen)
                .SingleOrDefaultAsync(c => c.HashedData == hashedData);
            return candidate;
        }

        public Task<List<Candidate>> GetPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("CandidateRepository: Fetching paged candidates - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            var candidates = _dbContext.Candidates
                 .Include(v => v.Citizen)
                 .OrderBy(v => v.Id)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .AsNoTracking()
                 .ToListAsync();
            return candidates;
        }

        public Task<int> CountAsync()
        {
            _logger.LogInformation("CandidateRepository: Counting total candidates");
            return _dbContext.Candidates.CountAsync();
        }

        public async Task<bool> IsCandidateExistByNationalIdAsync(string nationalId)
        {
            _logger.LogInformation("CandidateRepository: Checking if candidate exists by NationalId");
            return await _dbContext.Candidates.AnyAsync(c => c.Citizen.NationalId == nationalId);
        }
    }
}