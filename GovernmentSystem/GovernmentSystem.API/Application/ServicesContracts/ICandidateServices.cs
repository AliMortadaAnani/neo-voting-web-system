using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface ICandidateServices
    {
        Task<Result<CandidateResponseDTO>> GetByNationalIdAsync
            (GetCandidateRequestDTO request);
        Task<Result<List<CandidateResponseDTO>>> GetAllCandidatesAsync();
        Task<Result<CandidateResponseDTO>> AddCandidateAsync
            (CreateCandidateRequestDTO request);
        Task<Result<CandidateResponseDTO>> UpdateByNationalIdAsync
            (UpdateCandidateRequestDTO request);
        Task<Result<bool>> DeleteByNationalIdAsync
            (DeleteCandidateRequestDTO request);
        Task<Result<CandidateResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenCandidateRequestDTO request);

        Task<Result<bool>> UpdateCandidateIsRegisteredToTrueAsync(NeoVoting_CandidateIsRegisteredRequestDTO request);
        Task<Result<NeoVoting_CandidateResponseDTO>> GetCandidateForNeoVotingAsync(NeoVoting_GetCandidateRequestDTO request);
    }
}
