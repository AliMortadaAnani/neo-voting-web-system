using NeoVoting.Domain.Entities;
using System.Diagnostics.Metrics;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ISystemAuditLogRepository
    {
        
        Task<List<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken);

        Task<List<SystemAuditLog>> GetPagedSystemAuditLogsStoredProcAsync(int skip, int take, CancellationToken cancellationToken);

        Task<List<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int skip, int take, CancellationToken cancellationToken);

        Task<int> GetTotalSystemAuditLogsCountAsync(CancellationToken cancellationToken);

        Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken);

    }
}
