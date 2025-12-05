using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateProfileRepository
    {
        //test git
        Task<List<CandidateProfile>> GetAllCandidatesProfilesAsync(CancellationToken cancellationToken);

        Task<CandidateProfile?> GetCandidateProfileByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<CandidateProfile> AddCandidateProfileAsync(CandidateProfile candidateProfile, CancellationToken cancellationToken);



        void Update(CandidateProfile candidateProfile);

    }
}
