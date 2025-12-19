using System.Collections.Generic;
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

        public async Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken)
        {
            await _dbContext.SystemAuditLogs.AddAsync(log, cancellationToken);
            return log;
        }

        public async Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogByUserIdAsync(Guid UserId, CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                .Include(l => l.User)
                .Include(l => l.Election)
                .Where(l => l.UserId == UserId)
                .OrderByDescending(l => l.TimestampUTC)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                .Include(l => l.User)// user will be null when using stored proc
                .Include(l => l.Election)// election will be null when using stored proc
                .OrderByDescending(l => l.TimestampUTC)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                .Include(l => l.User)
                .Include(l => l.Election)
                .OrderByDescending(l => l.TimestampUTC)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalSystemAuditLogsCountAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs.CountAsync(cancellationToken);
        }
    }
}