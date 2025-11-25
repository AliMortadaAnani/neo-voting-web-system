using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Domain.Shared;


namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface IVoterServices
    {
        Task<Result<VoterResponseDTO>> GetByNationalIdAsync(GetVoterRequestDTO request);
        Task<Result<List<VoterResponseDTO>>> GetAllVotersAsync();
        Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request);
        Task<Result<VoterResponseDTO>> UpdateByNationalIdAsync(UpdateVoterRequestDTO request);
        Task<Result<bool>> DeleteByNationalIdAsync(DeleteVoterRequestDTO request);
        Task<Result<VoterResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenVoterRequestDTO request);

        Task<Result<NeoVoting_VoterResponseDTO>> UpdateHasVotedToTrueAsync(NeoVoting_VoterHasVotedRequestDTO request);
        Task<Result<NeoVoting_VoterResponseDTO>> UpdateVoterIsRegisteredToTrueAsync(NeoVoting_VoterIsRegisteredRequestDTO request);
        Task<Result<NeoVoting_VoterResponseDTO>> GetVoterForNeoVotingAsync(NeoVoting_GetVoterRequestDTO request);
        Task<Result<bool>> ResetAllVotedAsFalseAsync();
    }
}
