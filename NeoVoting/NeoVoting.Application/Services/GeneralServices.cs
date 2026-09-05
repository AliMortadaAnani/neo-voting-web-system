using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class GeneralServices : IGeneralServices
    {
        public Task<Result<Election_ResponseDTO>> GetActiveElectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<Poll_ResponseDTO>> GetActivePollAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<Election_ResponseDTO>> GetElectionByIdAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CandidateProfile_ResponseDTO>> GetPagedCandidateResultsForElectionAsync(int electionId, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Election_ResponseDTO>> GetPagedElectionsAsync(int? page, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ElectionVoteLog_ResponseDTO>> GetPagedElectionVoteLogsAsync(int electionId, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Poll_ResponseDTO>> GetPagedPollsAsync(int? page, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<PollVoteLog_ResponseDTO>> GetPagedPollVoteLogsAsync(int pollId, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Poll_ResponseDTO>> GetPollByIdAsync(int pollId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Poll_ResponseDTO>> GetPollResultsAsync(int pollId)
        {
            throw new NotImplementedException();
        }
    }
}