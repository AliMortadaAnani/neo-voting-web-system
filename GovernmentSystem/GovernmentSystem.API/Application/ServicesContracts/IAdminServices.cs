using GovernmentSystem.API.Application.RequestDTOs.AdminDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.AdminDTOs;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface IAdminServices
    {
        Task<Result<AuthResponse>> LoginAsync(LoginDTO loginDTO);

        Task<Result<string>> LogoutAsync();
    }
}