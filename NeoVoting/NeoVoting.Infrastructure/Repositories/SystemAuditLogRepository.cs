using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class SystemAuditLogRepository : ISystemAuditLog
    {
        private readonly ApplicationDbContext _dbContext;
        public SystemAuditLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SystemAuditLog?> GetSystemAuditLogByIdAsync(long id, CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<List<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                .Include(l => l.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken)
        {
            await _dbContext.SystemAuditLogs.AddAsync(log, cancellationToken);
            return log;
        }

        public void Update(SystemAuditLog log)
        {
            _dbContext.SystemAuditLogs.Update(log);
        }
    }
}