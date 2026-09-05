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

        Task<Result<PagedResult<Election_ResponseDTO>>> GetPagedElectionsAsync(int pageNumber, int pageSize);

        Task<Result<PagedResult<Poll_ResponseDTO>>> GetPagedPollsAsync(int pageNumber, int pageSize);

        Task<Result<PagedResult<ElectionVoteLog_ResponseDTO>>> GetPagedElectionVoteLogsAsync(int electionId,
            int pageNumber, int pageSize);

        Task<Result<PagedResult<PollVoteLog_ResponseDTO>>> GetPagedPollVoteLogsAsync(int pollId, int pageNumber, int pageSize);

        Task<Result<Poll_ResponseDTO>> GetPollResultsAsync(int pollId);

        Task<Result<PagedResult<CandidateProfile_ResponseDTO>>> GetPagedCandidateResultsForElectionAsync(int electionId, int pageNumber, int pageSize);
    }
}