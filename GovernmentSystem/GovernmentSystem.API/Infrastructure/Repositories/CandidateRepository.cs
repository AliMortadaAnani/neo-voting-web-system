using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.Shared;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public CandidateRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Candidate>> GetPagedCandidatesStoredProcAsync(int skip, int take)
        {
            //    return await _dbContext.Candidates
            //.FromSqlRaw("EXEC GetPagedCandidates @Skip, @Take",
            //    new SqlParameter("@Skip", skip),
            //    new SqlParameter("@Take", take))
            //.AsNoTracking() // <--- Performance optimization
            //.ToListAsync();

            //var stopwatch = Stopwatch.StartNew();

            var result = new List<Candidate>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("GetPagedCandidates", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Skip", skip);
                    command.Parameters.AddWithValue("@Take", take);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(MapCandidateFromReader(reader));
                        }
                    }
                }
            }
            //stopwatch.Stop();
            //Console.WriteLine($"ADO Duration: {stopwatch.ElapsedMilliseconds} ms");  // Or log it
            return result;
        }

        // Helper method for mapping
        private static Candidate MapCandidateFromReader(SqlDataReader reader)
        {
            return Candidate.FromAdoNet(
                reader.GetGuid(reader.GetOrdinal("Id")),
                reader.GetGuid(reader.GetOrdinal("NationalId")),
                reader.GetGuid(reader.GetOrdinal("NominationToken")),
                (GovernorateId)reader.GetInt32(reader.GetOrdinal("GovernorateId")),
                reader.GetString(reader.GetOrdinal("FirstName")),
                reader.GetString(reader.GetOrdinal("LastName")),
                DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))),
                reader.GetString(reader.GetOrdinal("Gender"))[0],
                reader.GetBoolean(reader.GetOrdinal("EligibleForElection")),
                reader.GetBoolean(reader.GetOrdinal("ValidToken")),
                reader.GetBoolean(reader.GetOrdinal("IsRegistered"))
            );
        }

        public async Task<int> GetTotalCandidatesCountAsync()
        {
            // Simple EF Core count is optimized enough.
            // It generates "SELECT COUNT(*) FROM Voters"
            return await _dbContext.Candidates.CountAsync();
        }

        public async Task<Candidate> AddCandidateAsync(Candidate candidate)
        {
            await _dbContext.Candidates.AddAsync(candidate);
            return candidate;
        }

        public async Task<List<Candidate>> GetAllCandidatesAsync()
        {
            return await _dbContext.Candidates.AsNoTracking().ToListAsync();
        }

        public async Task<Candidate?> GetCandidateByNationalIdAsync(Guid nationalId)
        {
            // FindAsync only works for Primary Keys. For other columns, use FirstOrDefault.
            return await _dbContext.Candidates
                .FirstOrDefaultAsync(v => v.NationalId == nationalId);
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