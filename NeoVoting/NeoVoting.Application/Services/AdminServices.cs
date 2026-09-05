using NeoVoting.Application.RequestDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class AdminServices : IAdminServices
    {
        public Task<Result<Election_ResponseDTO>> CompleteElectionAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Poll_ResponseDTO>> CompletePollAsync(int pollId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Election_ResponseDTO>> CreateElectionAsync(ElectionCreate_RequestDTO electionRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Poll_ResponseDTO>> CreatePollAsync(PollCreate_RequestDTO pollRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<SystemAuditLog_ResponseDTO>> GetPagedSystemAuditLogsAsync(SystemActionTypesEnum? actionTypesEnum, int? adminId, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Election_ResponseDTO>> StartElectionAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Poll_ResponseDTO>> StartPollAsync(int pollId)
        {
            throw new NotImplementedException();
        }
    }
}