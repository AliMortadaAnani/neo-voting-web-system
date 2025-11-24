using GovernmentSystem.API.Application.AdminDTOs;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface IAdminServices
    {
        Task<Result<AuthResponse>> LoginAsync(LoginDTO loginDTO);
        Task<Result<string>> LogoutAsync();
    }
}
