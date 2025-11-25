using GovernmentSystem.API.API.Filters;
using GovernmentSystem.API.Application.AdminDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ApiController
    {

        private readonly IAdminServices _adminServices;

        public AuthController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _adminServices.LoginAsync(request);
            return HandleResult(result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var result = await _adminServices.LogoutAsync();
            return HandleResult(result);
        }

        // Test Endpoint
        /*[HttpGet("check-status")]
        [Authorize] // <--- If Cookie is missing, this returns 401
        public IActionResult CheckStatus()
        {
            var name = User.Identity?.Name ?? "Unknown";
            return Ok($"You are logged in as {name}");
        }*/

        // Test Endpoint
        /*[HttpGet("test-api-key")]
        [ApiKeyAuth]
        public IActionResult testApiKe()
        {
            return Ok("Hello Caller!");
        }*/


        // Test Endpoint
        /*[HttpGet("test-ips")]
        public IActionResult testIps()
        {
            return Ok("Hello external IP!");
        }*/
    }
}
