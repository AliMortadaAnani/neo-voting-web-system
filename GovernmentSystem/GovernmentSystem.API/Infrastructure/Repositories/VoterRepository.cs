using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class VoterRepository : IVoterRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<VoterRepository> _logger;

        public VoterRepository(ApplicationDbContext dbContext, ILogger<VoterRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public void Add(Voter voter)
        {
            _logger.LogInformation("VoterRepository: Adding new voter");
            _dbContext.Voters.Add(voter);
        }

        public void Delete(Voter voter)
        {
            _logger.LogInformation("VoterRepository: Deleting voter");
            _dbContext.Voters.Remove(voter);
        }

        public Task<Voter?> GetVoterByNationalIdAsync(string nationalId)
        {
            _logger.LogInformation("VoterRepository: Fetching voter by NationalId");
            var voter = _dbContext.Voters
                .Include(v => v.Citizen)
                .SingleOrDefaultAsync(v => v.Citizen.NationalId == nationalId);
            return voter;
        }

        public Task<Voter?> GetVoterByHashedDataAsync(string hashedData)
        {
            _logger.LogInformation("VoterRepository: Fetching voter by HashedData");
            var voter = _dbContext.Voters
                .Include(v => v.Citizen)
                .SingleOrDefaultAsync(v => v.HashedData == hashedData);
            return voter;
        }

        public Task<List<Voter>> GetPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("VoterRepository: Fetching paged voters - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            var voters = _dbContext.Voters
                 .AsNoTracking()
                 .Include(v => v.Citizen)
                 .OrderBy(v => v.Id)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
            return voters;
        }

        public Task<int> CountAsync()
        {
            _logger.LogInformation("VoterRepository: Counting total voters");
            return _dbContext.Voters.CountAsync();
        }

        public async Task<bool> IsVoterExistByNationalIdAsync(string nationalId)
        {
            _logger.LogInformation("VoterRepository: Checking if voter exists by NationalId");
            return await _dbContext.Voters.AnyAsync(v => v.Citizen.NationalId == nationalId);
        }
    }
}