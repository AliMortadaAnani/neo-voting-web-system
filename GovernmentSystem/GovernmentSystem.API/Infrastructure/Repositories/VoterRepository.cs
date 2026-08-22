using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class VoterRepository : IVoterRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VoterRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Voter voter)
        {
            _dbContext.Voters.Add(voter);
        }

        public void Delete(Voter voter)
        {
            _dbContext.Voters.Remove(voter);
        }

        public Task<Voter?> GetVoterByNationalIdAsync(string nationalId)
        {
            var voter = _dbContext.Voters
                .Include(v => v.Citizen)
                .SingleOrDefaultAsync(v => v.Citizen.NationalId == nationalId);
            return voter;
        }

        public Task<Voter?> GetVoterByHashedDataAsync(string hashedData)
        {
            var voter = _dbContext.Voters
                .Include(v => v.Citizen)
                .SingleOrDefaultAsync(v => v.HashedData == hashedData);
            return voter;
        }

        public Task<List<Voter>> GetPagedAsync(int pageNumber, int pageSize)
        {
           var voters = _dbContext.Voters
                .Include(v => v.Citizen)
                .OrderBy(v => v.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            return voters;
        }

        public Task<int> CountAsync()
        {
            return _dbContext.Voters.CountAsync();
        }
    }
}