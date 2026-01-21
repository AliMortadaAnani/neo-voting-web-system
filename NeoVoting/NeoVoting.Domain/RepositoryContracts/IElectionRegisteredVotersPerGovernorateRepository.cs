using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionRegisteredVotersPerGovernorateRepository
    {
        // governorateId is nullable because it can represent the total registered voters across all governorates when null
        Task<ElectionRegisteredVotersPerGovernorate?> GetByElectionIdAndGovernorateIdAsync(Guid electionId, int? governorateId, CancellationToken cancellationToken);

        Task<ElectionRegisteredVotersPerGovernorate> AddAsync(ElectionRegisteredVotersPerGovernorate entity, CancellationToken cancellationToken);

    }
}
