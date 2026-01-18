using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionRegisteredVotersPerGovernorateRepository : IElectionRegisteredVotersPerGovernorateRepository
    {
        public Task<ElectionRegisteredVotersPerGovernorate> AddAsync(ElectionRegisteredVotersPerGovernorate entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ElectionRegisteredVotersPerGovernorate?> GetByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
