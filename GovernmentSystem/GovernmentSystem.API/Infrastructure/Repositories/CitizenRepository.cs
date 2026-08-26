using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class CitizenRepository : ICitizenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CitizenRepository> _logger;

        public CitizenRepository(ApplicationDbContext dbContext, ILogger<CitizenRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public void Add(Citizen citizen)
        {
            _logger.LogInformation("CitizenRepository: Adding new citizen");
            _dbContext.Citizens.Add(citizen);
        }

        public void Delete(Citizen citizen)
        {
            _logger.LogInformation("CitizenRepository: Deleting citizen");
            _dbContext.Citizens.Remove(citizen);
        }

        public Task<Citizen?> GetCitizenByNationalIdAsync(string nationalId)
        {
            _logger.LogInformation("CitizenRepository: Fetching citizen by NationalId");
            return _dbContext.Citizens.SingleOrDefaultAsync(c => c.NationalId == nationalId);
        }

        public Task<List<Citizen>> GetPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("CitizenRepository: Fetching paged citizens - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return _dbContext.Citizens
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<int> CountAsync()
        {
            _logger.LogInformation("CitizenRepository: Counting total citizens");
            return _dbContext.Citizens.CountAsync();
        }
    }
}