using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPollRepository
    {
        void Add(Poll poll);

        Task<bool> IsActivePollExistsAsync();

        Task<bool> IsPollNameExistsAsync(string pollName);

        Task<int> CountAsync();

        Task<List<Poll>> GetPagedAsync(int pageNumber, int pageSize);

        Task<Poll?> GetByIdAsync(int pollId);


        Task<Poll?> GetActivePollAsync();

        Task<bool> IsPollUpcomingPhaseAsync(int pollId);

        Task<bool> IsPollVotingPhaseAsync(int pollId);

        Task<bool> IsPollCompletedPhaseAsync(int pollId);
    }
}