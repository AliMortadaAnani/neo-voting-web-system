using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteChoiceRepository
    {
        void Add(VoteChoice voteChoice);

        Task<bool> IsVoteChoiceExistsByVoteIdAndCandidateProfileIdAsync(Guid voteId, int candidateProfileId);

        //vote count by candidate profile id(which includes election id and governorate id info)
        Task<int> GetCountOfTotalVoteChoicesByCandidateProfileIdAsync(int candidateProfileId);

        // winners per governorate id per election id
        Task<List<CandidateResultResponseEF_DTO>> GetTop5CandidatesProfilesPerGovernorateAsync(int electionId, GovernorateIdEnum governorate);

        Task<List<CandidateResultResponseEF_DTO>> GetPagedCandidatesProfilesResultsAsync(int electionId, GovernorateIdEnum? governorate, int pageNumber, int pageSize);

        Task<int> CountCandidatesProfilesResultsAsync(
            int electionId,
            GovernorateIdEnum? governorate);
    }
}