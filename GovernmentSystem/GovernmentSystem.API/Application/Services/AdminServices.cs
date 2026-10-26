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

        private readonly IHttpContextAccessor _httpContextAccessor;

       
        public AdminServices(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<AdminServices> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public string? CurrentUserName =>
       _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public async Task<Result<AuthResponse>> GetCurrentUserAsync()
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity;

            if (identity?.IsAuthenticated == true)
            {
                var username = identity.Name;
                _logger.LogInformation("Admin current user resolved successfully for username: {Username}", username);

                return Result<AuthResponse>.Success(new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Logged in successfully",
                    Username = username!,
                    Role = "Admin"
                });
            }

            _logger.LogWarning("Admin current user lookup failed because no authenticated admin identity was found.");
            //usually, authorization middleware should handle this, but in case it doesn't, we return an unauthorized error
            return Result<AuthResponse>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Admin_InvalidCredentials), "Admin is not authenticated."));
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginDTO loginDTO)
        {
            _logger.LogInformation("Admin login attempt initiated for username: {Username}", loginDTO.Username);

            var user = await _userManager.FindByNameAsync(loginDTO.Username!);
            if (user == null)
            {
                _logger.LogWarning("Admin login failed - user not found for username: {Username}", loginDTO.Username);
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

            _logger.LogWarning(
                "Admin login failed for username: {Username}. Sign-in result: {SignInResult}",
                user.UserName,
                result);
            return Result<AuthResponse>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Admin_InvalidCredentials), "Invalid admin credentials."));
        }

        public async Task<Result<string>> LogoutAsync()
        {
            _logger.LogInformation("Admin logout initiated for current authenticated user: {Username}", CurrentUserName);

            // This tells the browser to delete the cookie
            await _signInManager.SignOutAsync();

            _logger.LogInformation("Admin logout successful for user: {Username}", CurrentUserName ?? "unknown");
            return Result<string>.Success("Logout Successful");
        }
    }
}