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

        public AuthController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _adminServices.LoginAsync(request);
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var result = await _adminServices.LogoutAsync();
            return HandleResult(result);
        }
    }
}