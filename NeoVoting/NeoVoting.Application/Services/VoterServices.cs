using NeoVoting.Application.RequestDTOs.VoterDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class VoterServices : IVoterServices
    {
        public Task<Result<ElectionVoteLog_ResponseDTO>> CastVoteInElectionAsync(int electionId, Voter_Cast_In_Election_RequestDTO voter_Cast_In_Election_RequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<PollVoteLog_ResponseDTO>> CastVoteInPollAsync(int pollId, Voter_Cast_In_Poll_RequestDTO voter_Cast_In_Poll_RequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CandidateProfile_ResponseDTO>> GetPagedNominatedCandidatesProfilesForElectionAsync(int electionId, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }
    }
}