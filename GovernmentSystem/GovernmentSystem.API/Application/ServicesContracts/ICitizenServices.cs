using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface ICitizenServices
    {
        Task<Result<CitizenResponseDTO>> GetCitizenByNationalIdAsync(GetCitizenRequestDTO request);
        Task<Result<PagedResult<CitizenResponseDTO>>> GetCitizensPagedAsync(int pageNumber, int pageSize);
        Task<Result<CitizenResponseDTO>> AddCitizenAsync(CreateCitizenRequestDTO request);
        Task<Result<bool>> DeleteCitizenByNationalIdAsync(DeleteCitizenRequestDTO request);
        Task<Result<CitizenResponseDTO>> UpdateCitizenByNationalIdAsync(UpdateCitizenRequestDTO request);
        Task<Result<int>> GetCitizensTotalCountAsync();
    }
}