using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionRepository
    {
        void Add(Election election);

        Task<bool> IsActiveElectionExistsAsync();

        Task<bool> IsElectionNameExistsAsync(string electionName);

        Task<List<Election>> GetPagedAsync(int pageNumber, int pageSize);

        Task<int> CountAsync();

        Task<Election?> GetByIdAsync(int electionId);

        Task<Election?> GetByNameAsync(string electionName);

        Task<Election?> GetActiveElectionAsync();

        Task<bool> IsElectionUpcomingPhaseAsync(int electionId);

        Task<bool> IsElectionVotingPhaseAsync(int electionId);

        Task<bool> IsElectionCompletedPhaseAsync(int electionId);
    }
}