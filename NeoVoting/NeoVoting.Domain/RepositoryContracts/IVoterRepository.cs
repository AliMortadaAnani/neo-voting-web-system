using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoterRepository
    {
        Task<bool> IsVoterExistByVerificationHashAsync(string verificationHash);

        void Add(Voter voter);

        Task<Voter?> GetByUserIdAsync(int userId);
    }
}