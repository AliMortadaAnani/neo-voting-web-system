using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPollStatisticsRepository
    {
        Task<PollStatistics?> GetByPollIdAsync(int pollId);

        void Add(PollStatistics pollStatistics);
    }
}