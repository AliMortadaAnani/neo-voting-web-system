using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        Task<List<Candidate>> GetPagedAsync(int pageNumber, int pageSize);

        Task<int> CountAsync();//needed for pagination UX

        Task<Candidate?> GetCandidateByNationalIdAsync(string nationalId);

        Task<Candidate?> GetCandidateByHashedDataAsync(string hashedData);

        void Add(Candidate candidate);

        void Delete(Candidate candidate);
    }
}