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

        // Unified Paged Method (Handles All, by Action, by Admin, or both)
        public async Task<List<SystemAuditLog>> GetPagedAsync(
            SystemActionTypesEnum? actionType,
            int? adminId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.SystemAuditLogs.AsNoTracking();

            // Apply optional ActionType filter if provided
            if (actionType.HasValue)
            {
                query = query.Where(s => s.ActionType == actionType.Value);
            }

            // Apply optional AdminId filter if provided
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

        // Unified Count Method (Handles All, by Action, by Admin, or both)
        public async Task<int> CountAsync(
            SystemActionTypesEnum? actionType,
            int? adminId)
        {
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