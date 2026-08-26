using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPollStatisticsRepository
    {
        Task<PollStatistics?> GetByPollIdAsync(int pollId);
        Task<PollStatistics?> GetByPollNameAsync(string pollName);
        
   


        void Add(PollStatistics pollStatistics);
    }
}
