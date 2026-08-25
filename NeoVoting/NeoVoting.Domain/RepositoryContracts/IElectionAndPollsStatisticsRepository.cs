using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionAndPollsStatisticsRepository
    {
        // governorateId is nullable because it can represent the total registered voters across all governorates when null
        Task<ElectionAndPollStatistics?> GetByElectionIdAsync(int electionId);

        Task<ElectionAndPollStatistics?> GetByPollIdAsync(int pollId);

        Task<ElectionAndPollStatistics?> GetByElectionNameAsync(string electionName);

        Task<ElectionAndPollStatistics?> GetByPollNameAsync(string pollName);


        void Add(ElectionAndPollStatistics electionAndPollStatistics);

    }
}
