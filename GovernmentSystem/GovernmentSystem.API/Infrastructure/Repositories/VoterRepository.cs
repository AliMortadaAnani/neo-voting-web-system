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


        public async Task<List<Voter>> GetAllVotersAsync()
        {
            return await _dbContext.Voters.ToListAsync();
        }

        public async Task<Voter?> GetVoterByNationalIdAsync(Guid id)
        {
            // FindAsync only works for Primary Keys. For other columns, use FirstOrDefault.
            return await _dbContext.Voters
                .FirstOrDefaultAsync(v => v.NationalId == id);
        }

        public void Update(Voter voter)
        {
            _dbContext.Voters.Update(voter);
        }

        public void Delete(Voter voter)
        {
            _dbContext.Voters.Remove(voter);
        }

        public async Task ResetAllVotedFieldToFalse()
        {
            // This runs ONE SQL command: UPDATE Voters SET Voted = 0, ValidToken = 0
            await _dbContext.Voters
                .ExecuteUpdateAsync(setters => setters
                    // Set 'Voted' to false
                    .SetProperty(v => v.Voted, false)
                // Optional: If you want to make a nullable field NULL
                // .SetProperty(v => v.SomeNullableField, (string?)null) 
                );

            
        }
    }
}