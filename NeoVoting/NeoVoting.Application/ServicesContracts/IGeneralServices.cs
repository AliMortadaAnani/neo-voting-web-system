using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IGeneralServices
    {
        Task<Result<Election_ResponseDTO>> GetActiveElectionAsync();

        Task<Result<Poll_ResponseDTO>> GetActivePollAsync();

        Task<Result<Election_ResponseDTO>> GetElectionByIdAsync(int electionId);

        Task<Result<Poll_ResponseDTO>> GetPollByIdAsync(int pollId);

        Task<PagedResult<Election_ResponseDTO>> GetPagedElectionsAsync(int? page, int? pageSize);

        Task<PagedResult<Poll_ResponseDTO>> GetPagedPollsAsync(int? page, int? pageSize);

        Task<PagedResult<ElectionVoteLog_ResponseDTO>> GetPagedElectionVoteLogsAsync(int electionId,
            int? pageNumber, int? pageSize);

        Task<PagedResult<PollVoteLog_ResponseDTO>> GetPagedPollVoteLogsAsync(int pollId, int? pageNumber, int? pageSize);

        Task<Result<Poll_ResponseDTO>> GetPollResultsAsync(int pollId);

        Task<PagedResult<CandidateProfile_ResponseDTO>> GetPagedCandidateResultsForElectionAsync(int electionId, int? pageNumber, int? pageSize);
    }
}