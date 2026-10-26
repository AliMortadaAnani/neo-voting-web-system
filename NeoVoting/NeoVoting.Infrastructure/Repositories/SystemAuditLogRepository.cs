using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class SystemAuditLogRepository : ISystemAuditLogRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemAuditLogRepository> _logger;

        public SystemAuditLogRepository(ApplicationDbContext context, ILogger<SystemAuditLogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(SystemAuditLog log)
        {
            _logger.LogInformation("Adding audit log entry for admin {AdminId} with action {ActionType}.", log.AdminId, log.ActionType);
            _context.SystemAuditLogs.Add(log);
        }

        public async Task<List<SystemAuditLog>> GetPagedAsync(
            SystemActionTypesEnum? actionType,
            int? adminId,
            int pageNumber,
            int pageSize)
        {
            _logger.LogInformation("Fetching audit log page with action filter {ActionType}, admin filter {AdminId}, page {PageNumber}, size {PageSize}.", actionType, adminId, pageNumber, pageSize);
            var query = _context.SystemAuditLogs.AsNoTracking();

            if (actionType.HasValue)
            {
                query = query.Where(s => s.ActionType == actionType.Value);
            }

            if (adminId.HasValue)
            {
                query = query.Where(s => s.AdminId == adminId.Value);
            }

            return await query
                .OrderByDescending(s => s.TimestampUTC)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(
            SystemActionTypesEnum? actionType,
            int? adminId)
        {
            _logger.LogInformation("Counting audit logs with action filter {ActionType} and admin filter {AdminId}.", actionType, adminId);
            var query = _context.SystemAuditLogs.AsQueryable();

            if (actionType.HasValue)
            {
                query = query.Where(s => s.ActionType == actionType.Value);
            }

            if (adminId.HasValue)
            {
                query = query.Where(s => s.AdminId == adminId.Value);
            }

            return await query.CountAsync();
        }
    }
}