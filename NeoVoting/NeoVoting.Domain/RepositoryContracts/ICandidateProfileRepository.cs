using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateProfileRepository
    {
        
        Task<List<CandidateProfile>> GetAllCandidatesProfilesByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken);
        Task<List<CandidateProfile>> GetPagedCandidatesProfilesByElectionIdAsync(Guid ElectionId, int skip,int take,CancellationToken cancellationToken);

        Task<int> GetTotalCandidatesProfilesCountByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken);
        Task<CandidateProfile?> GetCandidateProfileByUserIdAndElectionIdAsync(Guid UserId,Guid ElectionId,CancellationToken cancellationToken);

        Task<CandidateProfile> AddCandidateProfileAsync(CandidateProfile candidateProfile, CancellationToken cancellationToken);



        void Update(CandidateProfile candidateProfile);

    }
}
