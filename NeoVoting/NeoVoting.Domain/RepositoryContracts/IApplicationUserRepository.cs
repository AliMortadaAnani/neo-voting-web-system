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
        Task<int> GetCountOfTotalVotersAsync(CancellationToken cancellationToken);
        Task<int> GetCountOfVotersByGovernorateIdAsync(int governorateId, CancellationToken cancellationToken);
        Task<int> GetCountOfVotersByGenderAsync(char gender, CancellationToken cancellationToken);
        Task<int> GetCountOfVotersByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken);
        Task<int> GetCountOfVotersByGovernorateAndGenderAsync(int governorateId, char gender, CancellationToken cancellationToken);
        Task<int> GetCountOfVotersByGovernorateAndAgeRangeAsync(int governorateId, int minAge, int maxAge, CancellationToken cancellationToken);
    }
}
