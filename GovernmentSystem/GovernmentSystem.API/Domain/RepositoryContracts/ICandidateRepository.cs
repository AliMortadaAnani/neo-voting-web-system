using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        Task<List<Candidate>> GetAllCandidatesAsync();

        Task<Candidate?> GetCandidateByNationalIdAsync(Guid id);

        Task<Candidate> AddCandidateAsync(Candidate candidate);


        // Add these back. They are simple, synchronous persistence operations.
        void Update(Candidate candidate);
        void Delete(Candidate candidate);
    }
}
