using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class CitizenRepository : ICitizenRepository
    {   
        private readonly ApplicationDbContext _dbContext;

        public CitizenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddCitizen(Citizen citizen)
        {
           _dbContext.Citizens.Add(citizen);
        }

        public void Delete(Citizen citizen)
        {
            _dbContext.Citizens.Remove(citizen);
        }

        public Task<Citizen?> GetCitizenByNationalIdAsync(string nationalId)
        {
            return _dbContext.Citizens.SingleOrDefaultAsync(c => c.NationalId == nationalId);
        }

        public Task<List<Citizen>> GetCitizensPagedAsync(int pageNumber, int pageSize)
        {
            return _dbContext.Citizens
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<int> GetCitizensTotalCountAsync()
        {
            return _dbContext.Citizens.CountAsync();
        }
    }
}
