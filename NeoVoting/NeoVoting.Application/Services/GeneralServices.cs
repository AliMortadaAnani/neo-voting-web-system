using NeoVoting.Application.ResponseDTOs;
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

        private readonly IVoteRepository _voteRepository;

        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IPollVoteRepository _pollVoteRepository;

        public GeneralServices(IElectionRepository electionRepository, IPollRepository pollRepository, IVoteRepository voteRepository, IPollVoteRepository pollVoteRepository, ICandidateProfileRepository candidateProfileRepository)
        {
            _electionRepository = electionRepository;
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
            _pollVoteRepository = pollVoteRepository;
            _candidateProfileRepository = candidateProfileRepository;
        }


        public async Task<Result<Election_ResponseDTO>> GetActiveElectionAsync()
        {
            var activeElection = await _electionRepository.GetActiveElectionAsync();
            if (activeElection == null)
            {
                return Result<Election_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.ActiveElectionNotFound), "No active election found."));
            }
            var electionResponseDTO = activeElection.ToElectionResponse();
            return Result<Election_ResponseDTO>.Success(electionResponseDTO);
        }

        public async Task<Result<Poll_ResponseDTO>> GetActivePollAsync()
        {
            var activePoll = await _pollRepository.GetActivePollAsync();
            if (activePoll == null)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.ActivePollNotFound), "No active poll found."));
            }
            var pollResponseDTO = activePoll.ToPollResponse();
            return Result<Poll_ResponseDTO>.Success(pollResponseDTO);
        }

        public async Task<Result<PagedResult<CandidateProfile_ResponseDTO>>> GetPagedCandidateResultsForElectionAsync(int electionId, int pageNumber, int pageSize)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<PagedResult<CandidateProfile_ResponseDTO>>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));
            }

            if(election.Status != StatusEnum.Completed)
            {
                return Result<PagedResult<CandidateProfile_ResponseDTO>>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.Election_NotComplete), "Election has not completed yet. Results are not available."));
            }

            if (pageNumber < 1)
            {
                return Result<PagedResult<CandidateProfile_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageNumber must be greater than 0."));
            }
            if (pageSize < 1)
            {

                return Result<PagedResult<CandidateProfile_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageSize must be greater than 0."));
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            // 3. Get total count
            int totalCount = await _candidateProfileRepository.CountByElectionIdAsync(electionId);

            var pagedLogs = await _voteRepository.GetPagedCandidatesProfilesResultsAsync(electionId, pageNumber, pageSize);


            var responseDTOs = pagedLogs.Select(log => log.ToCandidateProfileResultsResponse()).ToList();
            var pagedResult = new PagedResult<CandidateProfile_ResponseDTO>
            {
                Data = responseDTOs,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<CandidateProfile_ResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<PagedResult<Election_ResponseDTO>>> GetPagedElectionsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                return Result<PagedResult<Election_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageNumber must be greater than 0."));
            }
            if (pageSize < 1)
            {

                return Result<PagedResult<Election_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageSize must be greater than 0."));
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            // 3. Get total count
            int totalCount = await _electionRepository.CountAsync();

            var pagedLogs = await _electionRepository.GetPagedAsync(pageNumber, pageSize);


            var responseDTOs = pagedLogs.Select(log => log.ToElectionResponse()).ToList();
            var pagedResult = new PagedResult<Election_ResponseDTO>
            {
                Data = responseDTOs,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<Election_ResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<PagedResult<ElectionVoteLog_ResponseDTO>>> GetPagedElectionVoteLogsAsync(int electionId, int pageNumber, int pageSize)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<PagedResult<ElectionVoteLog_ResponseDTO>>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));

            }
            if (election.Status == StatusEnum.Upcoming)
            {
                return Result<PagedResult<ElectionVoteLog_ResponseDTO>>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.ElectionInvalidState), "Election has not started yet. Results are not available."));

            }
            if (pageNumber < 1)
            {
                return Result<PagedResult<ElectionVoteLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageNumber must be greater than 0."));
            }
            if (pageSize < 1)
            {

                return Result<PagedResult<ElectionVoteLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageSize must be greater than 0."));
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            // 3. Get total count
            int totalCount = await _voteRepository.CountByElectionIdAsync(electionId);

            var pagedLogs = await _voteRepository.GetPagedByElectionIdAsync(electionId, pageNumber, pageSize);



            var responseDTOs = pagedLogs.Select(log => new ElectionVoteLog_ResponseDTO
            {
                VoteId = log.Id,
                TimestampUTC = log.TimestampUTC
            }).ToList();


            var pagedResult = new PagedResult<ElectionVoteLog_ResponseDTO>
            {
                Data = responseDTOs,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<ElectionVoteLog_ResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<PagedResult<Poll_ResponseDTO>>> GetPagedPollsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                return Result<PagedResult<Poll_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageNumber must be greater than 0."));
            }
            if (pageSize < 1)
            {

                return Result<PagedResult<Poll_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageSize must be greater than 0."));
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            // 3. Get total count
            int totalCount = await _pollRepository.CountAsync();

            var pagedLogs = await _pollRepository.GetPagedAsync(pageNumber, pageSize);


            var responseDTOs = pagedLogs.Select(log => log.ToPollResponse()).ToList();
            var pagedResult = new PagedResult<Poll_ResponseDTO>
            {
                Data = responseDTOs,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<Poll_ResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<PagedResult<PollVoteLog_ResponseDTO>>> GetPagedPollVoteLogsAsync(int pollId, int pageNumber, int pageSize)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return Result<PagedResult<PollVoteLog_ResponseDTO>>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Poll_NotFound), "Poll not found."));

            }
            if (poll.Status == StatusEnum.Upcoming )
            {
                return Result<PagedResult<PollVoteLog_ResponseDTO>>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollInvalidState), "Poll has not started yet. Results are not available."));

            }
            if (pageNumber < 1)
            {
                return Result<PagedResult<PollVoteLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageNumber must be greater than 0."));
            }
            if (pageSize < 1)
            {

                return Result<PagedResult<PollVoteLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageSize must be greater than 0."));
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            // 3. Get total count
            int totalCount = await _pollVoteRepository.CountByPollIdAsync(pollId);

            var pagedLogs = await _pollVoteRepository.GetPagedByPollIdAsync(pollId, pageNumber, pageSize);



            var responseDTOs = pagedLogs.Select(log => new PollVoteLog_ResponseDTO
            {
                VoteId = log.Id,
                TimestampUTC = log.TimestampUTC
            }).ToList();

            var pagedResult = new PagedResult<PollVoteLog_ResponseDTO>
            {
                Data = responseDTOs,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<PollVoteLog_ResponseDTO>>.Success(pagedResult);
        }



        public async Task<Result<Poll_ResponseDTO>> GetPollResultsAsync(int pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Poll_NotFound), "Poll not found."));

            }
            if (poll.Status != StatusEnum.Completed)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollNotComplete), "Poll has not ended yet. Results are not available."));

            }

            var pollResults = await _pollVoteRepository.GetResultsAsyncByPollId(pollId);
            var responseDTO = poll.ToPollResponse(pollResults);

            return Result<Poll_ResponseDTO>.Success(responseDTO);

        }
    }
}