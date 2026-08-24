using GovernmentSystem.API.Application.RequestDTOs.AdminDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.AdminDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GovernmentSystem.API.API.Controllers
{
    [EnableRateLimiting("AuthLimiter")]
    public class AuthController : ApiController
    {
        private readonly IAdminServices _adminServices;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAdminServices adminServices, ILogger<AuthController> logger)
        {
            _adminServices = adminServices;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            _logger.LogInformation("Login attempt initiated for user");
            var result = await _adminServices.LoginAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("User login successful");
            else
                _logger.LogWarning("User login failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout initiated");
            var result = await _adminServices.LogoutAsync();
            if (result.IsSuccess)
                _logger.LogInformation("User logout successful");
            else
                _logger.LogWarning("User logout failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }
    }
}