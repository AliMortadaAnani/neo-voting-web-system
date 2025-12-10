using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class SystemAuditLogRepository : ISystemAuditLogRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public SystemAuditLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*public async Task<SystemAuditLog?> GetSystemAuditLogByIdAsync(long id, CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }*/




        public async Task<List<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                .AsNoTracking()
                .Include(l => l.User)
                .Include(l => l.Election)
                .ToListAsync(cancellationToken);
        }

        public async Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken)
        {
            await _dbContext.SystemAuditLogs.AddAsync(log, cancellationToken);
            return log;
        }


        public async Task<int> GetTotalSystemAuditLogsCountAsync(CancellationToken cancellationToken)
        {
            // Simple EF Core count is optimized enough.
            // It generates "SELECT COUNT(*) FROM ..."
            return await _dbContext.SystemAuditLogs.CountAsync(cancellationToken);
        }

        public async Task<List<SystemAuditLog>> GetPagedSystemAuditLogsStoredProcAsync(int skip, int take, CancellationToken cancellationToken)
        {
            // 1. Use the DbSet to get Entities
            var result = await _dbContext.SystemAuditLogs
                .FromSqlRaw("EXEC GetPagedSystemAuditLogs @Skip, @Take",
                    new SqlParameter("@Skip", skip),
                    new SqlParameter("@Take", take))

                // 2. IMPORTANT: Since these are Entities, EF tracks them by default.
                // We MUST use AsNoTracking() for read-only performance.
                .AsNoTracking()
                //.Include(l => l.User) // user will be null when using stored proc
                //.Include(l => l.Election) // election will be null when using stored proc
                .ToListAsync(cancellationToken);

            //stopwatch.Stop();
            //Console.WriteLine($" Ef Core Stored Proc: {stopwatch.ElapsedMilliseconds} ms");  // Or log it
            return result;
        }


        public async Task<List<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int skipCount, int pageSize, CancellationToken cancellationToken)
        {
            // 1. Validation (Optional but recommended) in Service Layer not here
            //if (pageNumber < 1) pageNumber = 1;
            //if (pageSize < 1) pageSize = 10;

            //// 2. Calculate how many items to skip
            //// Example: Page 1 -> Skip 0. Page 2 -> Skip 10.
            //int skipCount = (pageNumber - 1) * pageSize;

            // 3. Run the Query
            return await _dbContext.SystemAuditLogs
                // A. Include the User so the navigation property isn't null
                .Include(l => l.User)
                .Include(l => l.Election)
                // B. Order is REQUIRED for pagination to work consistently.
                // We usually want the newest logs first.
                .OrderByDescending(l => l.TimestampUTC)

                // C. Skip the previous pages
                .Skip(skipCount)

                // D. Take the current page size
                .Take(pageSize)

                // E. Execute
                .ToListAsync(cancellationToken);
        }

    }
}