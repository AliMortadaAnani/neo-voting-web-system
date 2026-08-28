using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IVoterServices
    {
        Task<Result<VoterCastVote_ResponseDTO>> VoterCastVoteAsync(Guid electionId, VoterCastVote_RequestDTO request, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<CandidateProfile_ResponseDTO>>> GetPagedCandidatesByElectionIdAsync(Guid electionId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<CandidateProfile_ResponseDTO>>> GetPagedCandidatesByElectionIdAndGovernorateIdAsync
            (Guid electionId, int governorateId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Result<PublicVoteLog_ResponseDTO>> GetPublicVoteLogByVoteIdAsync(Guid electionId, Guid voteId, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<PublicVoteLog_ResponseDTO>>> GetPagedPublicVoteLogsByElectionIdAsync
            (Guid electionId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Result<IReadOnlyList<PublicVoteLog_ResponseDTO>>> GetPagedPublicVoteLogsByElectionIdAndGovernorateIdAsync
            (Guid electionId, int governorateId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}