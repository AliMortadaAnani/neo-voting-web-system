using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {       
        Task<bool> IsVoterExistByVerificationHashAsync(string verificationHash);

        void Add(Voter voter);

        Task<Voter?> GetByUserIdAsync(int userId);
    }
}