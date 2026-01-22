using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IAdminServices
    {
        Task<Result<Election_ResponseDTO>> AddElectionAsync(ElectionAdd_RequestDTO requestDTO,CancellationToken cancellationToken);

        Task<Result<Election_ResponseDTO>> UpdateElectionStatusAsync(Guid electionId, ElectionStatusEnum newStatus, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsAsync
            (int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsByActionTypeAsync
            (SystemActionTypesEnum systemActionTypesEnum,int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsByElectionIdAsync
            (Guid electionId,int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetSystemAuditLogsByUserIdAsync
            (Guid userID, CancellationToken cancellationToken);


    }
}
