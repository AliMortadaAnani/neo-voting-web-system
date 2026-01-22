using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionStatisticsRepository
    {
        // governorateId is nullable because it can represent the total registered voters across all governorates when null
        Task<ElectionStatistics?> GetByElectionIdAndGovernorateIdAsync(Guid electionId, int? governorateId, CancellationToken cancellationToken);

        Task<ElectionStatistics> AddAsync(ElectionStatistics entity, CancellationToken cancellationToken);

    }
}
