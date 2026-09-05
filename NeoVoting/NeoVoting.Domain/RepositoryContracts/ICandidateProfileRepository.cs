using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateProfileRepository
    {
        void Add(CandidateProfile candidateProfile);
        
        Task<bool> IsCandidateProfileExistsInElectionAsync(int candidateProfileId, int electionId);
        Task<CandidateProfile?> GetByCandidateIdAndElectionIdAsync(int candidateId, int electionId);

        Task<bool> IsCandidateProfileExistsByCandidateIdAndElectionIdAsync(int candidateId, int electionId);

        Task<int> CountByElectionIdAsync(
            int electionId);

        Task<List<CandidateProfile>> GetPagedByElectionIdAsync(
     int electionId,
     int pageNumber,
     int pageSize);
    }
}