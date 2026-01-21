using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class GeneralServices : IGeneralServices
    {
        public Task<Result<IReadOnlyList<Election_ResponseDTO>>> GetAllCompletedElectionsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<Election_ResponseDTO>>> GetAllElectionsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ElectionCompletedStatistics_ResponseDTO>> GetCompletedElectionStatsByIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ElectionCompletedStatistics_ResponseDTO>> GetCompletedElectionStatsByIdPerGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Election_ResponseDTO>> GetCurrentActiveElectionAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ElectionCurrentActiveStatistics_ResponseDTO>> GetCurrentActiveElectionStatsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
