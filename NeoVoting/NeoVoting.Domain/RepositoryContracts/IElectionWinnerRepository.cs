using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionWinnerRepository
    {
        Task<IReadOnlyList<ElectionWinner>> GetAllWinnersByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken);

        Task<ElectionWinner> AddWinnerAsync(ElectionWinner winner, CancellationToken cancellationToken);

        void Update(ElectionWinner winner);
    }
}