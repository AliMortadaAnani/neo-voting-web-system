using GovernmentSystem.API.Application.AdminDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.Services
{
    public class AdminServices : IAdminServices
    {
        public AdminServices()
        {
        }

        public Task<Result<AuthResponse>> LoginAsync(LoginDTO loginDTO)
        {
            return Task.FromResult(Result<AuthResponse>.Failure(Error.NullValue));
        }

        public Task<Result<bool>> LogoutAsync()
        {
            return Task.FromResult(Result<bool>.Failure(Error.NullValue));
        }
    }
}
