using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ISystemAuditLogRepository
    {
        Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken);

        Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsAsync(int skip, int take, CancellationToken cancellationToken);
        Task<int> GetTotalSystemAuditLogsCountAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsByActionTypeAsync(SystemActionTypesEnum systemAction, int skip, int take, CancellationToken cancellationToken);
        Task<int> GetTotalSystemAuditLogsCountByActionTypeAsync(SystemActionTypesEnum systemAction, CancellationToken cancellationToken);

        

        Task<IReadOnlyList<SystemAuditLog>> GetPagedSystemAuditLogsByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken);
        Task<int> GetTotalSystemAuditLogsCountByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);

        Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogsByUserIdAsync(Guid userId, CancellationToken cancellationToken);


        //for testing and debugging purposes only
        Task<IReadOnlyList<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken);
    }

}