using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface IVoterServices
    {
        Task<Result<VoterResponseDTO>> GetVoterByNationalIdAsync(GetVoterRequestDTO request);
        Task<Result<PagedResult<VoterResponseDTO>>> GetVotersPagedAsync(int pageNumber, int pageSize);
        Task<Result<VoterVerifyResponseDTO>> VerifyVoterCredentialsAsync(GetVoterVerificationRequestDTO request);
        Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request);
        Task<Result<bool>> DeleteVoterByNationalIdAsync(DeleteVoterRequestDTO request);
        Task<Result<VoterResponseDTO>> GenerateNewVotingTokenByNationalIdAsync(UpdateVoterRequestDTO request);
        Task<Result<int>> GetVotersTotalCountAsync();
    }
}