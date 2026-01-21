using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class AdminServices : IAdminServices
    {
        public Task<Result<Election_ResponseDTO>> AddElectionAsync(ElectionAdd_RequestDTO requestDTO, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsAsync(int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsByActionTypeAsync(SystemActionTypesEnum systemActionTypesEnum, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetSystemAuditLogsByUserIdAsync(Guid userID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
