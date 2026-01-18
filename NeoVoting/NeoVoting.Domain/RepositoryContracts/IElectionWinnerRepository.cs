using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionWinnerRepository
    {

        Task<ElectionWinner> AddWinnerAsync(ElectionWinner winner, CancellationToken cancellationToken);
        Task<IReadOnlyList<ElectionWinner>> GetAllWinnersByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken);// Not paged since we expect only 25 winners per election

        Task<IReadOnlyList<ElectionWinner>> GetAllWinnersByElectionIdAndGovernorateIdAsync(Guid electionId,int governorateId, CancellationToken cancellationToken);// Not paged since we expect only 5 winners per election

        void Update(ElectionWinner winner);
    }
}