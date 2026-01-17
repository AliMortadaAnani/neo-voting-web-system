using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
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

        public async Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogsByUserIdAsync(Guid UserId, CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                
                
                .Where(l => l.UserId == UserId)
                .OrderByDescending(l => l.TimestampUTC)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
               
                .OrderByDescending(l => l.TimestampUTC)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.SystemAuditLogs
                
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

        public Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsByActionTypeAsync(SystemActionTypesEnum systemAction, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalSystemAuditLogsCountByActionTypeAsync(SystemActionTypesEnum systemAction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsByUserIdAsync(Guid userId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalSystemAuditLogsCountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalSystemAuditLogsCountByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}