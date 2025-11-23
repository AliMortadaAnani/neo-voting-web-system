using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;
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

        public async Task<Voter> AddVoterAsync(Voter voter)
        {
            await _dbContext.Voters.AddAsync(voter);
            return voter;
        }

        public void Delete(Voter voter)
        {
            _dbContext.Voters.Remove(voter);
        }

        public async Task<List<Voter>> GetAllVotersAsync()
        {
            return await _dbContext.Voters.ToListAsync();
        }

        public async Task<Voter?> GetVoterByIdAsync(Guid id)
        {
            return await _dbContext.Voters.FindAsync(id);
        }

        public void Update(Voter voter)
        {
            _dbContext.Voters.Update(voter);
        }
    }
}