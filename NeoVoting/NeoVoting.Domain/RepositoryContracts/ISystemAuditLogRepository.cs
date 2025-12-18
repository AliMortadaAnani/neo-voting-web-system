using NeoVoting.Domain.Entities;
using System.Diagnostics.Metrics;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ISystemAuditLogRepository
    {
        Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogByUserIdAsync(Guid UserId, CancellationToken cancellationToken);
        Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int skip, int take, CancellationToken cancellationToken);

        Task<int> GetTotalSystemAuditLogsCountAsync(CancellationToken cancellationToken);

        Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken);

    }
}
