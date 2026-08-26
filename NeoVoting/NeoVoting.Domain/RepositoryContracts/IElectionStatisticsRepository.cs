using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
