using GovernmentSystem.API.Application.RequestDTOs.AdminDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.AdminDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using Microsoft.AspNetCore.Identity;

namespace GovernmentSystem.API.Application.ResponseDTOs.Admin
{
    public class AdminServices : IAdminServices
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminServices> _logger;

        public AdminServices(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AdminServices> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginDTO loginDTO)
        {
            _logger.LogInformation("Admin login attempt initiated");
            var user = await _userManager.FindByNameAsync(loginDTO.Username!);
            if (user == null)
            {
                _logger.LogWarning("Admin login failed - user not found");
                return Result<AuthResponse>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Admin_InvalidCredentials), "Invalid admin credentials."));
            }
            // This method creates the Set-Cookie header
            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginDTO.Password!,
                isPersistent: true, //better be false for secutity reasons - not current scope
                lockoutOnFailure: false); // not current scope

            if (result.Succeeded)
            {
                _logger.LogInformation("Admin login successful for user: {Username}", user.UserName);
                return Result<AuthResponse>.Success(new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Login successful",
                    Username = user.UserName!,
                    Role = "Admin"
                });
            }

            _logger.LogWarning("Admin login failed - password sign in unsuccessful");
            return Result<AuthResponse>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Admin_InvalidCredentials), "Invalid admin credentials."));
        }

        public async Task<Result<string>> LogoutAsync()
        {
            _logger.LogInformation("Admin logout initiated");
            // This tells the browser to delete the cookie
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Admin logout successful");
            return Result<string>.Success("Logout Successful");
        }
    }
}