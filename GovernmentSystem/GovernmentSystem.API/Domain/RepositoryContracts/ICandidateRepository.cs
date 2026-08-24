using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        Task<Candidate?> GetCandidateByNationalIdAsync(string nationalId);
        Task<List<Candidate>> GetPagedAsync(int pageNumber, int pageSize);
        Task<Candidate?> GetCandidateByHashedDataAsync(string hashedData);
        Task<bool> IsCandidateExistByNationalIdAsync(string nationalId);
        Task<int> CountAsync();//needed for pagination UX
        void Add(Candidate candidate);
        void Delete(Candidate candidate);
    }
}