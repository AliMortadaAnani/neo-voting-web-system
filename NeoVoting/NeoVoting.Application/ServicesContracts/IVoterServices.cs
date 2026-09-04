using NeoVoting.Application.RequestDTOs.VoterDTOs;
using NeoVoting.Application.ResponseDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IVoterServices
    {
        Task<PagedResult<CandidateProfile_ResponseDTO>> GetPagedNominatedCandidatesProfilesForElectionAsync(int electionId, int? pageNumber, int? pageSize);

        Task<Result<ElectionVoteLog_ResponseDTO>> CastVoteInElectionAsync(int electionId, Voter_Cast_In_Election_RequestDTO voter_Cast_In_Election_RequestDTO);

        Task<Result<PollVoteLog_ResponseDTO>> CastVoteInPollAsync(int pollId, Voter_Cast_In_Poll_RequestDTO voter_Cast_In_Poll_RequestDTO);

        Task<Result<ElectionVoteLog_ResponseDTO>> TrackVoteInElectionAsync(int electionId, Voter_TrackVote_RequestDTO voter_TrackVote_RequestDTO);

        Task<Result<PollVoteLog_ResponseDTO>> TrackVoteInPollAsync(int pollId, Voter_TrackVote_RequestDTO voter_TrackVote_RequestDTO);

    }
}