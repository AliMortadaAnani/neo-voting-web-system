using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class SystemAuditLogRepository : ISystemAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public SystemAuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(SystemAuditLog log)
        {
            _context.SystemAuditLogs.Add(log);
        }

        public async Task<List<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int pageNumber, int pageSize)
        {
            return await _context.SystemAuditLogs
                .AsNoTracking()
                .OrderByDescending(s => s.TimestampUTC)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.SystemAuditLogs.CountAsync();
        }

        public async Task<List<SystemAuditLog>> GetPagedByActionTypeAsync(SystemActionTypesEnum systemAction, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.SystemAuditLogs
                .AsNoTracking()
                .Where(s => s.ActionType == systemAction)
                .OrderByDescending(s => s.TimestampUTC)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByActionTypeAsync(SystemActionTypesEnum systemAction)
        {
            return await _context.SystemAuditLogs.CountAsync(s => s.ActionType == systemAction);
        }

        public async Task<List<SystemAuditLog>> GetPagedByAdminIdAsync(int adminId, int pageNumber, int pageSize)
        {
            return await _context.SystemAuditLogs
                .AsNoTracking()
                .Where(s => s.AdminId == adminId)
                .OrderByDescending(s => s.TimestampUTC)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByAdminIdAsync(int adminId)
        {
            return await _context.SystemAuditLogs.CountAsync(s => s.AdminId == adminId);
        }
    }
}