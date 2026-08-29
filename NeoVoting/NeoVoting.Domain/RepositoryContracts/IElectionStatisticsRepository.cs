using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionStatisticsRepository
    {
        Task<ElectionStatistics?> GetByElectionIdAsync(int electionId, GovernorateIdEnum? governorate);

        void Add(ElectionStatistics electionAndPollStatistics);
    }
}