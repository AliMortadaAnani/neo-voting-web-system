using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        Task<bool> IsCandidateExistByVerificationHashAsync(string verificationHash);

        Task<Candidate?> GetByVerificationHashAsync(string verificationHash);

        void Add(Candidate candidate);

        Task<Candidate?> GetByUserIdAsync(int userId);
    }
}