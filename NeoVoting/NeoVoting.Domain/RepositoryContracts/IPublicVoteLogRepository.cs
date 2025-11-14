using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPublicVoteLogRepository
    {
        Task<PublicVoteLog?> GetPublicVoteLogByIdAsync(long logId, CancellationToken cancellationToken);
        Task<List<PublicVoteLog>> GetAllPublicVoteLogsAsync(CancellationToken cancellationToken);
        Task<PublicVoteLog> AddPublicVoteLogAsync(PublicVoteLog log, CancellationToken cancellationToken);

    }
}
