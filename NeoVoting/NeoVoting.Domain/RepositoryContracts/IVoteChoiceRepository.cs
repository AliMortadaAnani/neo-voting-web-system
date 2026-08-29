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
        Task<List<CandidateResultResponseDTO>> GetTop5CandidatesProfilesPerGovernorate(int electionId, GovernorateIdEnum governorate);

        Task<List<CandidateResultResponseDTO>> GetPagedCandidatesProfilesResultsPerGovernorate(int electionId, GovernorateIdEnum governorate, int pageNumber, int pageSize);

        Task<List<CandidateResultResponseDTO>> GetPagedCandidatesProfilesResults(int electionId, int pageNumber, int pageSize);
    }
}