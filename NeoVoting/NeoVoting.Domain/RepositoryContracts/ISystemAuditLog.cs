using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ISystemAuditLog
    {
        Task<SystemAuditLog?> GetSystemAuditLogByIdAsync(long id, CancellationToken cancellationToken);
        Task<List<SystemAuditLog>> GetAllSystemAuditLogsAsync(CancellationToken cancellationToken);
        Task<SystemAuditLog> AddSystemAuditLogAsync(SystemAuditLog log, CancellationToken cancellationToken);
        void Update(SystemAuditLog log);
    }
}
