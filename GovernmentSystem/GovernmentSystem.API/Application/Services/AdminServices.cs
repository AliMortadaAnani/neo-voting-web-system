using GovernmentSystem.API.Application.AdminDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace GovernmentSystem.API.Application.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminServices(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.Username!);
            if (user == null)
            {
                return Result<AuthResponse>.Failure(Error.Unauthorized("Admin.NotValid", "Invalid admin credentials."));
            }
            // This method creates the Set-Cookie header
            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginDTO.Password!,
                isPersistent: true, //better be false for secutity reasons
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Result<AuthResponse>.Success(new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Login successful",
                    Username = user.UserName!,
                    Role = "Admin"
                });
            }

            return Result<AuthResponse>.Failure(Error.Unauthorized("Admin.NotValid", "Invalid admin credentials."));
        }

        public async Task<Result<string>> LogoutAsync()
        {
            // This tells the browser to delete the cookie
            await _signInManager.SignOutAsync();
            return Result<string>.Success("Logout Successful");
        }
    }
}
