using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPollStatisticsRepository
    {
        Task<PollStatistics?> GetByPollIdAsync(int pollId);

        Task<PollStatistics?> GetByPollNameAsync(string pollName);

        void Add(PollStatistics pollStatistics);
    }
}