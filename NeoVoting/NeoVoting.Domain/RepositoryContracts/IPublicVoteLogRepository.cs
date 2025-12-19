using System.Collections.Generic;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPublicVoteLogRepository
    {
        Task<PublicVoteLog?> GetPublicVoteLogByVoteIdAsync(Guid VoteId, CancellationToken cancellationToken);
        Task<IReadOnlyList<PublicVoteLog>> GetAllPublicVoteLogsByElectionIdAsync(Guid ElectionId,CancellationToken cancellationToken);
        Task<IReadOnlyList<PublicVoteLog>> GetPagedPublicVoteLogsByElectionIdAsync(Guid ElectionId, int skip, int take,CancellationToken cancellationToken);

        Task<int> GetTotalPublicVoteLogsCountByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken);
        Task<PublicVoteLog> AddPublicVoteLogAsync(PublicVoteLog log, CancellationToken cancellationToken);

    }
}
