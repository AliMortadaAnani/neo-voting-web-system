using NeoVoting.Application.RequestDTOs.VoterDTOs;
using NeoVoting.Application.ResponseDTOs.CandidateDTOs;
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
        private readonly IElectionRepository _electionRepository;

        private readonly IPollRepository _pollRepository;

        private readonly IVoteRepository _voteRepository;

        private readonly ICandidateProfileRepository _candidateProfileRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IVoteChoiceRepository _voteChoiceRepository;

        private readonly ICurrentUserServices _currentUserServices;

        private readonly IVoterRepository _voterRepository;

        private readonly IPollVoteRepository _pollVoteRepository;

        private readonly IPollAnswerRepository _pollAnswerRepository;

        public VoterServices(IElectionRepository electionRepository, IPollRepository pollRepository, IVoteRepository voteRepository, ICandidateProfileRepository candidateProfileRepository, IUnitOfWork unitOfWork, IVoteChoiceRepository voteChoiceRepository, ICurrentUserServices currentUserServices, IVoterRepository voterRepository, IPollVoteRepository pollVoteRepository, IPollAnswerRepository pollAnswerRepository)
        {
            _electionRepository = electionRepository;
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _unitOfWork = unitOfWork;
            _voteChoiceRepository = voteChoiceRepository;
            _currentUserServices = currentUserServices;
            _voterRepository = voterRepository;
            _pollVoteRepository = pollVoteRepository;
            _pollAnswerRepository = pollAnswerRepository;
        }
        public async Task<Result<ElectionVoteLog_ResponseDTO>> CastVoteInElectionAsync(int electionId, Voter_Cast_In_Election_RequestDTO voter_Cast_In_Election_RequestDTO)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);

            var vote = Vote.Create(electionId, (GovernorateIdEnum)_currentUserServices.Governorate!);

            

            foreach (var selectedCandidateProfileId in voter_Cast_In_Election_RequestDTO.SelectedCandidateProfileIds!)
            {
                var voteChoice = VoteChoice.Create(selectedCandidateProfileId);
                vote.VoteChoices.Add(voteChoice);
            }

            _voteRepository.Add(vote);

            await _unitOfWork.SaveChangesAsync();

            var response = new ElectionVoteLog_ResponseDTO
            {
                VoteId = vote.Id,
                TimestampUTC = vote.TimestampUTC,
                ElectionId = electionId,
                ElectionName = election.Name,
                GovernorateId = (GovernorateIdEnum)_currentUserServices.Governorate!
            };

            return Result<ElectionVoteLog_ResponseDTO>.Success(response);
        }

        public async Task<Result<PollVoteLog_ResponseDTO>> CastVoteInPollAsync(int pollId, Voter_Cast_In_Poll_RequestDTO dto)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);

            var pollvoteChoice = PollVote.Create(pollId, (int)dto.SelectedPollAnswerId!);

             _pollVoteRepository.Add(pollvoteChoice);

            await _unitOfWork.SaveChangesAsync();

            var response = new PollVoteLog_ResponseDTO
            {
                VoteId = pollvoteChoice.Id,
                TimestampUTC = pollvoteChoice.TimestampUTC,
                PollId = pollId,
                PollName = poll.Name
            };

            return Result<PollVoteLog_ResponseDTO>.Success(response);
        }

        public async Task<PagedResult<CandidateProfile_ResponseDTO>> GetPagedNominatedCandidatesProfilesForElectionAsync(int electionId, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ElectionVoteLog_ResponseDTO>> TrackVoteInElectionAsync(int electionId, Voter_TrackVote_RequestDTO voter_TrackVote_RequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<PollVoteLog_ResponseDTO>> TrackVoteInPollAsync(int pollId, Voter_TrackVote_RequestDTO voter_TrackVote_RequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}