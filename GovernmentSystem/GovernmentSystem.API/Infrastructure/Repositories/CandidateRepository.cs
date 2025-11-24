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

        public async Task<Candidate> AddCandidateAsync(Candidate candidate)
        {
            await _dbContext.Candidates.AddAsync(candidate);
            return candidate;
        }


        public async Task<List<Candidate>> GetAllCandidatesAsync()
        {
            return await _dbContext.Candidates.ToListAsync();
        }

        public async Task<Candidate?> GetCandidateByNationalIdAsync(Guid id)
        {
            // FindAsync only works for Primary Keys. For other columns, use FirstOrDefault.
            return await _dbContext.Candidates
                .FirstOrDefaultAsync(v => v.NationalId == id);
        }

        public void Delete(Candidate candidate)
        {
            _dbContext.Candidates.Remove(candidate);
        }

        public void Update(Candidate candidate)
        {
            _dbContext.Candidates.Update(candidate);
        }
    }
}
