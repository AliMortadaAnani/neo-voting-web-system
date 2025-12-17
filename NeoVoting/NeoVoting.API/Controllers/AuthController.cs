using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using System.Security.Claims;
namespace NeoVoting.API.Controllers
{
    [Route("api/[controller]")]
   // [AllowAnonymous]
    public class AuthController : ApiController
    {
        private readonly IAuthServices _authService;
        public AuthController(IAuthServices authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(Registration_ResetPassword_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] RegisterVoterDTO request)
        {
            var result = await _authService.RegisterVoterAsync(request);
            return HandleResult(result);
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _authService.LoginAsync(request);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var claimsUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                          var dto = new LogoutDTO { Id = Guid.Parse(claimsUserId) }; // not client-supplied
            
            var result = await _authService.LogoutAsync(dto);
            return HandleResult(result);
        }
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return HandleResult(result);
        }

    }
}
