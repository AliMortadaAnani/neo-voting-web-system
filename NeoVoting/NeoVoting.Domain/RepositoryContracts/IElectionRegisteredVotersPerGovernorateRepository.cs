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
        Task<ElectionRegisteredVotersPerGovernorate?> GetByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken);

        Task<ElectionRegisteredVotersPerGovernorate> AddAsync(ElectionRegisteredVotersPerGovernorate entity, CancellationToken cancellationToken);

    }
}
