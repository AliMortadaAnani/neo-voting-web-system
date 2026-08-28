using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionStatisticsRepository
    {
        Task<ElectionStatistics?> GetByElectionIdAsync(int electionId);

        Task<ElectionStatistics?> GetByElectionNameAsync(string electionName);

        Task<ElectionStatistics?> GetByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate);

        Task<ElectionStatistics?> GetByElectionNameAndGovernorateAsync(string electionName, GovernorateIdEnum governorate);

        void Add(ElectionStatistics electionAndPollStatistics);
    }
}