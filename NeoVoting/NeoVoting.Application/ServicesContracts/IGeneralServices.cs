using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface IGeneralServices
    {
        Task<Result<IReadOnlyList<Election_ResponseDTO>>> GetAllElectionsAsync(CancellationToken cancellationToken);
        Task<Result<IReadOnlyList<Election_ResponseDTO>>> GetAllCompletedElectionsAsync(CancellationToken cancellationToken);

        Task<Result<ElectionCompletedStatistics_ResponseDTO>> GetCompletedElectionStatsByIdAsync(Guid electionId, CancellationToken cancellationToken);

        Task<Result<ElectionCompletedStatistics_ResponseDTO>> GetCompletedElectionStatsByIdPerGovernorateIdAsync(Guid electionId,int governorateId, CancellationToken cancellationToken);

        Task<Result<ElectionCurrentActiveStatistics_ResponseDTO>> GetCurrentActiveElectionStatsAsync(CancellationToken cancellationToken);

        Task<Result<Election_ResponseDTO>> GetCurrentActiveElectionAsync
            (CancellationToken cancellationToken);


    }
}
