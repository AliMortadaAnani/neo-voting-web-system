using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface IVoterServices
    {
        Task<Result<VoterResponseDTO>> GetByNationalIdAsync(GetVoterRequestDTO request);

        Task<Result<List<VoterResponseDTO>>> GetAllVotersAsync();

        Task<Result<List<VoterResponseDTO>>> GetPaginatedVotersAsync(int PageNumber, int PageSize);

        Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request);

        Task<Result<VoterResponseDTO>> UpdateByNationalIdAsync(UpdateVoterRequestDTO request);

        Task<Result<bool>> DeleteByNationalIdAsync(DeleteVoterRequestDTO request);

        Task<Result<VoterResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenVoterRequestDTO request);

        
    }
}