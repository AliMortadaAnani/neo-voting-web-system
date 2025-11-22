using GovernmentSystem.API.Domain.Shared;
using GovernmentSystem.Application.RequestDTOs;
using GovernmentSystem.Application.ResponseDTOs;

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
        Task<Result<CandidateResponseDTO>> DeleteByNationalIdAsync
            (DeleteCandidateRequestDTO request);
        Task<Result<CandidateResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenCandidateRequestDTO request);

        Task<Result<NeoVoting_CandidateResponseDTO>> UpdateCandidateIsRegisteredFieldAsync(NeoVoting_CandidateIsRegisteredRequestDTO request);
        Task<Result<NeoVoting_CandidateResponseDTO>> GetCandidateForNeoVotingAsync(NeoVoting_GetCandidateRequestDTO request);
    }
}
