using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionWinnerRepository
    {
        void Add(ElectionWinner winner);

        Task<bool> IsCandidateProfileWinnerExistByElectionIdAsync(int electionId, int candidateProfileId);

        Task<List<CandidateResultResponseEF_DTO>> GetPagedWinnersByElectionIdAsync(
    int electionId,
    GovernorateIdEnum? governorate,
    int pageNumber,
    int pageSize);


        Task<int> CountWinnersByElectionIdAsync(
            int electionId,
            GovernorateIdEnum? governorate);
    }
}