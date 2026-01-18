using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IApplicationUserRepository
    {
        Task<int> GetTotalVotersCountAsync(CancellationToken cancellationToken);
        Task<int> GetVotersCountByGovernorateIdAsync(int governorateId, CancellationToken cancellationToken);
        Task<int> GetVotersCountByGenderAsync(char gender, CancellationToken cancellationToken);
        Task<int> GetVotersCountByAgePhaseAsync(int minAge, int maxAge, CancellationToken cancellationToken);
        Task<int> GetVotersCountByGovernorateAndGenderAsync(int governorateId, char gender, CancellationToken cancellationToken);
        Task<int> GetVotersCountByGovernorateAndAgePhaseAsync(int governorateId, int minAge, int maxAge, CancellationToken cancellationToken);
    }
}
