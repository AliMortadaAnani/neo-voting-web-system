using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateProfileRepository
    {

        Task<CandidateProfile> AddCandidateProfileAsync(CandidateProfile candidateProfile, CancellationToken cancellationToken);

        Task<bool> IsCandidateProfileExistsByUserIdAndElectionIdAsync(Guid userId, Guid electionId, CancellationToken cancellationToken);

        Task<CandidateProfile?> GetCandidateProfileByUserIdAndElectionIdAsync(Guid userId, Guid electionId, CancellationToken cancellationToken);

        Task<IReadOnlyList<CandidateProfile>> GetPagedCandidatesProfilesByElectionIdAsync(Guid electionId, int skip, int take, CancellationToken cancellationToken);

        Task<int> GetCountOfTotalCandidatesProfilesByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);

        Task<IReadOnlyList<CandidateProfile>> GetPagedCandidatesProfilesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int skip, int take, CancellationToken cancellationToken);

        Task<int> GetCountOfTotalCandidatesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken);

        void Update(CandidateProfile candidateProfile);

        //for testing and debugging purposes only
        Task<IReadOnlyList<CandidateProfile>> GetAllCandidatesProfilesByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);

    }
}