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

        public  async Task<Candidate?> GetCandidateByNationalIdAsync(string nationalId)
        {
            _logger.LogInformation("CandidateRepository: Fetching candidate by NationalId");
            var candidate = await _dbContext.Candidates
                .Include(c => c.Citizen)
                .SingleOrDefaultAsync(c => c.Citizen.NationalId == nationalId);
            return candidate;
        }

        public async Task<Candidate?> GetCandidateByHashedDataAsync(string hashedData)
        {
            _logger.LogInformation("CandidateRepository: Fetching candidate by HashedData");
            var candidate = await _dbContext.Candidates
                .Include(c => c.Citizen)
                .SingleOrDefaultAsync(c => c.HashedData == hashedData);
            return candidate;
        }

        public async Task<List<Candidate>> GetPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("CandidateRepository: Fetching paged candidates - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            var candidates = await _dbContext.Candidates
                 .AsNoTracking()
                 .Include(v => v.Citizen)
                 .OrderBy(c => c.Citizen.LastName)
                 .ThenBy(c => c.Citizen.FirstName)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
            return candidates;
        }

        public async Task<int> CountAsync()
        {
            _logger.LogInformation("CandidateRepository: Counting total candidates");
            return await _dbContext.Candidates.CountAsync();
        }

        public async Task<bool> IsCandidateExistByNationalIdAsync(string nationalId)
        {
            _logger.LogInformation("CandidateRepository: Checking if candidate exists by NationalId");
            return await _dbContext.Candidates.AnyAsync(c => c.Citizen.NationalId == nationalId);
        }
    }
}