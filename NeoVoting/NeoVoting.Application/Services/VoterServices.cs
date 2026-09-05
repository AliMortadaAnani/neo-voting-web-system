using NeoVoting.Application.RequestDTOs.VoterDTOs;
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
    public class VoterServices : IVoterServices
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IElectionRepository _electionRepository;

        private readonly IPollRepository _pollRepository;

        private readonly ICandidateProfileRepository _candidateProfileRepository;

        private readonly IVoteRepository _voteRepository;

        private readonly IPollVoteRepository _pollVoteRepository;
        private readonly IEventParticipationRepository _eventParticipationRepository;
        private readonly ICurrentUserServices _currentUserServices;

        public VoterServices(IUnitOfWork unitOfWork, IElectionRepository electionRepository, IPollRepository pollRepository, ICandidateProfileRepository candidateProfileRepository,
            IVoteRepository voteRepository, IPollVoteRepository pollVoteRepository, ICurrentUserServices currentUserServices, IEventParticipationRepository eventParticipationRepository)
        {
            _unitOfWork = unitOfWork;
            _electionRepository = electionRepository;
            _pollRepository = pollRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _voteRepository = voteRepository;
            _pollVoteRepository = pollVoteRepository;
            _currentUserServices = currentUserServices;
            _eventParticipationRepository = eventParticipationRepository;
        }

        public async Task<Result<ElectionVoteLog_ResponseDTO>> CastVoteInElectionAsync(int electionId, Voter_Cast_In_Election_RequestDTO voter_Cast_In_Election_RequestDTO)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<ElectionVoteLog_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));
            }

            if (election.Status != StatusEnum.Voting)
            {
                return Result<ElectionVoteLog_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.ElectionInvalidState), "Election is not in voting phase."));
            }



            if(! await _candidateProfileRepository.IsCandidateProfileExistsInElectionAsync((int)voter_Cast_In_Election_RequestDTO.SelectedCandidateProfileId!, electionId))
            {
                return Result<ElectionVoteLog_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.CandidateProfile_NotFound), "Selected candidate profile not found in this election."));
            }

            int currentUserId = (int)_currentUserServices.AccountId!;

            if(await _eventParticipationRepository.HasVoterVotedByVoterIdAndElectionIdAsync(currentUserId, electionId))
            {
                return Result<ElectionVoteLog_ResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyVoted), "User had already participated in this election."));
            }

            

            var vote = Vote.Create
                (
                electionId, 
                (int)voter_Cast_In_Election_RequestDTO.SelectedCandidateProfileId!
                );

            _voteRepository.Add(vote);

            var participation = EventParticipation.CreateForElection(currentUserId, electionId);
            _eventParticipationRepository.Add(participation);

            await _unitOfWork.SaveChangesAsync();

            var addedVote = await _voteRepository.GetByIdAsync(vote.Id);

            if(addedVote == null)
            {
                return Result<ElectionVoteLog_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Vote creation failed."));
            }

            var responseDTO = new ElectionVoteLog_ResponseDTO
            {
                VoteId = addedVote.Id,
                TimestampUTC = addedVote.TimestampUTC
            };

            return Result<ElectionVoteLog_ResponseDTO>.Success(responseDTO);

        }

        public async Task<Result<PollVoteLog_ResponseDTO>> CastVoteInPollAsync(int pollId, Voter_Cast_In_Poll_RequestDTO voter_Cast_In_Poll_RequestDTO)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return Result<PollVoteLog_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Poll_NotFound), "Poll not found."));
            }

            if (poll.Status != StatusEnum.Voting)
            {
                return Result<PollVoteLog_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollInvalidState), "Poll is not in voting phase."));
            }

            if(!await _pollVoteRepository.IsPollAnswerExistByIdInPoll((int)voter_Cast_In_Poll_RequestDTO.SelectedPollAnswerId!, pollId))
            {
                return Result<PollVoteLog_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.PollAnswer_NotFound), "Selected poll answer not found in this poll."));
            }
            int currentUserId = (int)_currentUserServices.AccountId!;

            if (await _eventParticipationRepository.HasVoterVotedByVoterIdAndPollIdAsync(currentUserId, pollId))
            {
                return Result<PollVoteLog_ResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyVoted), "User had already participated in this poll."));
            }

            var vote = PollVote.Create
                (
                pollId,
                (int)voter_Cast_In_Poll_RequestDTO.SelectedPollAnswerId!
                );

            _pollVoteRepository.Add(vote);

            var participation = EventParticipation.CreateForPoll(currentUserId, pollId);
            _eventParticipationRepository.Add(participation);

            await _unitOfWork.SaveChangesAsync();

            var addedVote = await _pollVoteRepository.GetByIdAsync(vote.Id);

            if (addedVote == null)
            {
                return Result<PollVoteLog_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Vote creation failed."));
            }

            var responseDTO = new PollVoteLog_ResponseDTO
            {
                VoteId = addedVote.Id,
                TimestampUTC = addedVote.TimestampUTC
            };

            return Result<PollVoteLog_ResponseDTO>.Success(responseDTO);

        }

        public async Task<Result<PagedResult<CandidateProfile_ResponseDTO>>> GetPagedNominatedCandidatesProfilesForElectionAsync(int electionId, int pageNumber, int pageSize)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<PagedResult<CandidateProfile_ResponseDTO>>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));
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

            var pagedLogs = await _candidateProfileRepository.GetPagedByElectionIdAsync(electionId, pageNumber, pageSize);


            var responseDTOs = pagedLogs.Select(log => log.ToCandidateProfileResponse(null)).ToList();
            var pagedResult = new PagedResult<CandidateProfile_ResponseDTO>
            {
                Data = responseDTOs,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<CandidateProfile_ResponseDTO>>.Success(pagedResult);
        }
    }
}