using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionWinnerRepository
    {
        Task<ElectionWinner?> GetWinnerByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<ElectionWinner>> GetAllWinnersAsync(CancellationToken cancellationToken);
        Task<ElectionWinner> AddWinnerAsync(ElectionWinner winner, CancellationToken cancellationToken);
        void Update(ElectionWinner winner);
    }
}
