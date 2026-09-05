using NeoVoting.Application.RequestDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IAdminServices
    {
        Task<Result<Election_ResponseDTO>> CreateElectionAsync(ElectionCreate_RequestDTO electionRequestDTO);

        Task<Result<Poll_ResponseDTO>> CreatePollAsync(PollCreate_RequestDTO pollRequestDTO);

        Task<Result<Election_ResponseDTO>> StartElectionAsync(int electionId);

        Task<Result<Poll_ResponseDTO>> StartPollAsync(int pollId);

        Task<Result<Election_ResponseDTO>> CompleteElectionAsync(int electionId);

        Task<Result<Poll_ResponseDTO>> CompletePollAsync(int pollId);

        Task<Result<PagedResult<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsAsync(SystemActionTypesEnum? actionTypesEnum, int? adminId, int pageNumber, int pageSize);
    }
}