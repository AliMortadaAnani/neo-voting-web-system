using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        Task<List<Candidate>> GetAllCandidatesAsync();

        Task<List<Candidate>> GetPagedCandidatesStoredProcAsync(int skip, int take);

        Task<int> GetTotalCandidatesCountAsync();

        Task<Candidate?> GetCandidateByNationalIdAsync(Guid nationalId);

        Task<Candidate> AddCandidateAsync(Candidate candidate);

        void Update(Candidate candidate);

        void Delete(Candidate candidate);
    }
}