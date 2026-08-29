using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionWinnerRepository
    {
        void Add(ElectionWinner winner);

        Task<bool> IsCandidateProfileWinnerExistByElectionIdAsync(int electionId, int candidateProfileId);

        Task<List<CandidateResultResponseDTO>> GetAllWinnersByElectionIdAsync(int electionId);// Not paged since we expect only 25 winners per election

        Task<List<CandidateResultResponseDTO>> GetAllWinnersByElectionIdAndGovernorateAsync
            (int election, GovernorateIdEnum governorate);// Not paged since we expect only 5 winners per election
    }
}