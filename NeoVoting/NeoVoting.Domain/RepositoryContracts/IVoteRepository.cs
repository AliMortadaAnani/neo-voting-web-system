using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        Task<Vote?> GetVoteByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Vote>> GetAllVotesAsync(CancellationToken cancellationToken);
        Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken);

    }
}
