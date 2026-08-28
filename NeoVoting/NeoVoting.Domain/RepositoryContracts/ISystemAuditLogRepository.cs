using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ISystemAuditLogRepository
    {
        void Add(SystemAuditLog log);

        Task<List<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int pageNumber, int pageSize);

        Task<int> CountAsync();

        Task<List<SystemAuditLog>> GetPagedByActionTypeAsync(SystemActionTypesEnum systemAction, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<int> CountByActionTypeAsync(SystemActionTypesEnum systemAction);

        Task<List<SystemAuditLog>> GetPagedByAdminIdAsync(int adminId, int pageNumber, int pageSize);

        Task<int> CountByAdminIdAsync(int adminId);
    }
}