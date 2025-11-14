using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ISystemAuditLogRepository
    {
        Task<SystemAuditLog?> GetSystemAuditLogByIdAsync(long id, CancellationToken cancellationToken);
        Task<List<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken);
        Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken);

    }
}
