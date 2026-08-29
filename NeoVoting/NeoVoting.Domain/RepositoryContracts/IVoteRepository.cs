using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        void Add(Vote vote);

        Task<bool> IsVoteChoicesForVoteEqualFive(Vote vote);

        Task<Vote?> GetByVoteId(Guid voteId);

        Task<List<Vote>> GetPagedByElectionIdAsync(int electionId,GovernorateIdEnum? governorate, int pageNumber, int pageSize);

        Task<int> CountByElectionIdAsync (int electionId, GovernorateIdEnum? governorate);
    }
}