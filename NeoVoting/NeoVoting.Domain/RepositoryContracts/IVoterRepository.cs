using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        Task<int> CountAsync();
        Task<int> CountByGovernorateAsync(GovernorateIdEnum governorate);
        Task<int> CountByGenderAsync(char gender);
        Task<int> CountsByAgeRangeAsync(int minAge, int maxAge);
        Task<int> CountByGovernorateAndGenderAsync(GovernorateIdEnum governorate, char gender);
        Task<int> CountByGovernorateAndAgeRangeAsync(GovernorateIdEnum governorate, int minAge, int maxAge);

        Task<bool> IsVoterExistByVerificationHashAsync(string verificationHash);
        Task<Voter?> GetByVerificationHashAsync(string verificationHash);

        
        void Add(Voter voter);
    }

}
