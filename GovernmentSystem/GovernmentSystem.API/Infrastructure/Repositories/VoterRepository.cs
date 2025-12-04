using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.Shared;
using GovernmentSystem.API.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class VoterRepository : IVoterRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;        // NEW for ADO Read operations

        public VoterRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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
            //Console.WriteLine($" Ef Core Stored Proc: {stopwatch.ElapsedMilliseconds} ms");  // Or log it
            return result;
            /*var result = new List<Voter>();

            // 1. Create Connection (Using 'using' ensures it closes automatically)
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // 2. Setup Command
                using (var command = new SqlCommand("GetPagedVoters", connection))
                {
                    // CRITICAL: Tell ADO this is a Stored Procedure (Performance boost)
                    command.CommandType = CommandType.StoredProcedure;

                    // 3. Add Parameters
                    command.Parameters.AddWithValue("@Skip", skip);
                    command.Parameters.AddWithValue("@Take", take);

                    // 4. Execute Reader
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // 5. Read Data
                        while (await reader.ReadAsync())
                        {
                            result.Add(MapVoterFromReader(reader));
                        }
                    }
                }
            }

            return result;*/
        }

        // Helper to keep the main method clean
        private static Voter MapVoterFromReader(SqlDataReader reader)
        {
            return Voter.FromAdoNet(

               // Use GetOrdinal for fastest performance (avoids string lookup every row)
               reader.GetGuid(reader.GetOrdinal("Id")),
               reader.GetGuid(reader.GetOrdinal("NationalId")),
               reader.GetGuid(reader.GetOrdinal("VotingToken")),
               (GovernorateId)reader.GetInt32(reader.GetOrdinal("GovernorateId")),
                reader.GetString(reader.GetOrdinal("FirstName")),
                reader.GetString(reader.GetOrdinal("LastName")),
                DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))),
                reader.GetString(reader.GetOrdinal("Gender"))[0], // String to Char
                reader.GetBoolean(reader.GetOrdinal("EligibleForElection")),
                reader.GetBoolean(reader.GetOrdinal("ValidToken")),
                reader.GetBoolean(reader.GetOrdinal("IsRegistered")),
                reader.GetBoolean(reader.GetOrdinal("Voted"))
            );
        }

        public async Task<int> GetTotalVotersCountAsync()
        {
            // Simple EF Core count is optimized enough.
            // It generates "SELECT COUNT(*) FROM Voters"
            return await _dbContext.Voters.CountAsync();
        }

        public async Task<Voter> AddVoterAsync(Voter voter)
        {
            await _dbContext.Voters.AddAsync(voter);
            return voter;
        }

        public async Task<List<Voter>> GetAllVotersAsync()
        {
            // var stopwatch = Stopwatch.StartNew();
            var result = await _dbContext.Voters
                .AsNoTracking()
                .Take(100)
                .ToListAsync();
            //stopwatch.Stop();
            //Console.WriteLine($"EF core LINQ Duration: {stopwatch.ElapsedMilliseconds} ms");  // Or log it
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