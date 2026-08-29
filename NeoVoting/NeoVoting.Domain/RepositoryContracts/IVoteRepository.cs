using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        void Add(Vote vote);

        Task<bool> IsVoteChoicesForVoteEqualFive(Vote vote);

        Task<Vote?> GetByVoteId(Guid voteId);

        Task<List<Vote>> GetPagedByElectionIdAsync(int electionId, int pageNumber, int pageSize);
    }
}