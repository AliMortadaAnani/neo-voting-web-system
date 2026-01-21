using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class VoterServices : IVoterServices
    {
        public Task<Result<IReadOnlyList<CandidateProfile_ResponseDTO>>> GetPagedCandidatesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<CandidateProfile_ResponseDTO>>> GetPagedCandidatesByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<PublicVoteLog_ResponseDTO>>> GetPagedPublicVoteLogsByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<PublicVoteLog_ResponseDTO>>> GetPagedPublicVoteLogsByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<PublicVoteLog_ResponseDTO>> GetPublicVoteLogByVoteIdAsync(Guid electionId, Guid voteId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<VoterCastVote_ResponseDTO>> VoterCastVoteAsync(Guid electionId, VoterCastVote_RequestDTO request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
