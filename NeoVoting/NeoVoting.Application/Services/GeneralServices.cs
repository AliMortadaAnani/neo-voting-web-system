using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class GeneralServices : IGeneralServices
    {
        private readonly IElectionRepository _electionRepository;
        private readonly IPollRepository _pollRepository;

        private readonly IElectionStatisticsRepository _electionStatisticsRepository;

        private readonly IPollStatisticsRepository _pollStatisticsRepository;

        public GeneralServices(IElectionRepository electionRepository, IPollRepository pollRepository, IElectionStatisticsRepository electionStatisticsRepository, IPollStatisticsRepository pollStatisticsRepository)
        {
            _electionRepository = electionRepository;
            _pollRepository = pollRepository;
            _electionStatisticsRepository = electionStatisticsRepository;
            _pollStatisticsRepository = pollStatisticsRepository;
        }
        public Task<Result<Election_ResponseDTO>> GetActiveElectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<Poll_ResponseDTO>> GetActivePollAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<ElectionStatistics>> GetCompletedElectionStatisticsAsync(int electionId, GovernorateIdEnum governorate)
        {
            var statistics = await _electionStatisticsRepository.GetByElectionIdAsync(electionId, governorate);

            if(statistics == null)
            {
                return Result<ElectionStatistics>.Failure(Error.NotFound("Statistics not found", "The requested election statistics were not found."));
            }

            

            return Result<ElectionStatistics>.Success(statistics);
        }

        public async Task<Result<PollStatistics>> GetCompletedPollStatisticsAsync(int pollId)
        {
          var stats = await _pollStatisticsRepository.GetByPollIdAsync(pollId);
            if(stats == null)
            {
                return Result<PollStatistics>.Failure(Error.NotFound("Statistics not found", "The requested poll statistics were not found."));
            }
            return Result<PollStatistics>.Success(stats);
        }

        public Task<Result<Election_ResponseDTO>> GetElectionByIdAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CandidateResults_ResponseDTO>> GetPagedCandidateResultsForElectionAsync(int electionId, GovernorateIdEnum governorate, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Election_ResponseDTO>> GetPagedElectionsAsync(int? page, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<ElectionVoteLog_ResponseDTO>> GetPagedElectionVoteLogsAsync(int electionId, GovernorateIdEnum governorate, int? pageNumber, int? pageSize)
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
    }
}