using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        void Add(Vote vote);

        Task<List<Vote>> GetPagedByElectionIdAsync(int electionId, int pageNumber, int pageSize);

        Task<int> CountByElectionIdAsync(int electionId);

        Task<List<CandidateProfileWithVotesDto>> GetPagedCandidatesProfilesResultsAsync(int electionId, int pageNumber, int pageSize);
    }
}