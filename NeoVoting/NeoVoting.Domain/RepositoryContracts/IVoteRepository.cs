using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        Task<Vote?> GetVoteByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Vote>> GetAllVotesByElectionIdAsync(Guid ElectionId,CancellationToken cancellationToken);
        Task<IReadOnlyList<Vote>> GetPagedVotesByElectionIdAsync(Guid ElectionId,int skip,int take,CancellationToken cancellationToken);
        Task<int> GetTotalVotesCountByElectionIdAsync(Guid ElectionId,CancellationToken cancellationToken);
        Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken);

        void Delete(Vote vote);//in case Voted flag in Government System was not updated correctly to true

    }
}
