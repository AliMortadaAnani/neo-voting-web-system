using GovernmentSystem.API.Domain.Entities;

namespace GovernmentSystem.API.Domain.RepositoryContracts
{
    public interface ICandidateRepository
    {
        Task<List<Candidate>> GetCandidatesPagedAsync(int pageNumber, int pageSize);

        Task<int> GetCandidatesTotalCountAsync();//needed for pagination UX

        Task<Candidate?> GetCandidateByNationalIdAsync(Guid nationalId);

        void AddCandidate(Candidate candidate);

        void Delete(Candidate candidate);
    }
}