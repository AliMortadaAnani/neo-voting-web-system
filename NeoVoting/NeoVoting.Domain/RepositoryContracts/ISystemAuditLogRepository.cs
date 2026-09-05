using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ISystemAuditLogRepository
    {
        void Add(SystemAuditLog log);

        Task<List<SystemAuditLog>> GetPagedAsync(
            SystemActionTypesEnum? actionType, int? adminId,
            int pageNumber, int pageSize);

        Task<int> CountAsync(
            SystemActionTypesEnum? actionType, int? adminId
            );
    }
}