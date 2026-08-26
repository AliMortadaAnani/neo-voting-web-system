using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateProfileRepository
    {

        void Add(CandidateProfile candidateProfile);

        Task<CandidateProfile?> GetByCandidateIdAndElectionIdAsync(int candidateId, int electionId);
        Task<bool> IsCandidateProfileExistsByCandidateIdAndElectionIdAsync(int candidateId, int electionId);

        Task<List<CandidateProfile>> GetPagedByElectionIdAndGovernorateAsync
            (int electionId,GovernorateIdEnum governorate, int pageNumber, int pageSize);

        Task<int> CountByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate);

        // for stats

        Task<int> CountByElectionIdAsync(int electionId);
       // Task<int> CountByGovernorateAsync(GovernorateIdEnum governorate); done above
        Task<int> CountByElectionIdAndGenderAsync(int electionId, char gender);
        Task<int> CountsByElectionIdAndAgeRangeAsync(int electionId, int minAge, int maxAge);
        Task<int> CountByElectionIdAndGovernorateAndGenderAsync(int electionId, GovernorateIdEnum governorate, char gender);
        Task<int> CountByElectionIdAndGovernorateAndAgeRangeAsync(int electionId, GovernorateIdEnum governorate, int minAge, int maxAge);


    }
}