using NeoVoting.Domain.Entities;
using System.Threading;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface ICandidateProfileRepository
    {
        
        Task<List<CandidateProfile>> GetAllCandidatesProfilesAsync(CancellationToken cancellationToken);

        Task<CandidateProfile?> GetCandidateProfileByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<CandidateProfile> AddCandidateProfileAsync(CandidateProfile candidateProfile, CancellationToken cancellationToken);


       
        void Update(CandidateProfile candidateProfile);

    }
}
