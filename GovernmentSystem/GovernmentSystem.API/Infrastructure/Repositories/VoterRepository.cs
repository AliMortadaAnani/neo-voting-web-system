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

        public async Task<List<Voter>> GetPagedVotersStoredProcAsync(int skip, int take)
        {
            // var stopwatch = Stopwatch.StartNew();

            // 1. Use the DbSet to get Entities
            var result = await _dbContext.Voters
                .FromSqlRaw("EXEC GetPagedVoters @Skip, @Take",
                    new SqlParameter("@Skip", skip),
                    new SqlParameter("@Take", take))

                // 2. IMPORTANT: Since these are Entities, EF tracks them by default.
                // We MUST use AsNoTracking() for read-only performance.
                .AsNoTracking()

                .ToListAsync();

            //stopwatch.Stop();

            return result;
        }

        public async Task<int> GetTotalVotersCountAsync()
        {
            // Simple EF Core count is optimized enough.
            // It generates "SELECT COUNT(*) FROM Voters"
            //No AsNoTracking needed for Count
            return await _dbContext.Voters.CountAsync();
        }

        public async Task<Voter> AddVoterAsync(Voter voter)
        {
            //Tracking is needed for Add
            await _dbContext.Voters.AddAsync(voter);
            return voter;
        }

        public async Task<List<Voter>> GetAllVotersAsync()
        {
            var result = await _dbContext.Voters
                .AsNoTracking()
                //.Take(100)// Just to limit the result for performance
                .ToListAsync();

            return result;
        }

        public async Task<Voter?> GetVoterByNationalIdAsync(Guid nationalId)
        {
            // FindAsync only works for Primary Keys. For other columns, use FirstOrDefault.
            return await _dbContext.Voters
                .FirstOrDefaultAsync(v => v.NationalId == nationalId);
        }

        public void Update(Voter voter)
        {
            _dbContext.Voters.Update(voter);
        }

        public void Delete(Voter voter)
        {
            _dbContext.Voters.Remove(voter);
        }

        public async Task ResetAllVotedFieldToFalse()// we used bulk update because we have large number of records to update (not single record)
        {
            // This runs ONE SQL command: UPDATE Voters SET Voted = 0, ValidToken = 0,...
            await _dbContext.Voters
                .ExecuteUpdateAsync(setters => setters
                    // Set 'Voted' to false
                    .SetProperty(v => v.Voted, false)
                );
        }
    }
}