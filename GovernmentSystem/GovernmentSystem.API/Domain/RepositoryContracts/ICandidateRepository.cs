using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        Task<List<Candidate>> GetAllCandidates();

        Task<Candidate?> GetCandidateById(Guid id);

        Task<Candidate> AddCandidate(Candidate candidate);


        // Add these back. They are simple, synchronous persistence operations.
        void Update(Candidate candidate);
        void Delete(Candidate candidate);
    }
}
