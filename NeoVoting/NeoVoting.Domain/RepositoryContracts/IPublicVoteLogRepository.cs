using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPublicVoteLogRepository
    {

        Task<PublicVoteLog> AddPublicVoteLogAsync(PublicVoteLog log, CancellationToken cancellationToken);

        Task<IReadOnlyList<PublicVoteLog>> GetPagedPublicVoteLogsByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken);

        Task<int> GetCountOfTotalPublicVoteLogsByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);

        Task<IReadOnlyList<PublicVoteLog>> GetPagedPublicVoteLogsByElectionIdAndGovernorateIdAsync(Guid electionId,int governorateId, int skip, int take, CancellationToken cancellationToken);

        Task<int> GetCountOfTotalPublicVoteLogsByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken);

        Task<PublicVoteLog?> GetPublicVoteLogByVoteIdAsync(Guid voteId, CancellationToken cancellationToken);


        //for testing and debugging purposes only
        Task<IReadOnlyList<PublicVoteLog>> GetAllPublicVoteLogsByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);
    }
}