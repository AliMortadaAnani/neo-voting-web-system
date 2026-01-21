using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using System.Security.Claims;

namespace NeoVoting.API.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Register([FromBody] Register_ResetPassword_VoterOrCandidate_RequestDTO request)
        {
            var result = await _authService.RegisterVoterOrCandidateAsync(request,Domain.Enums.RoleTypesEnum.Voter);
            return HandleResult(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(Authentication_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] Login_RequestDTO request)
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
            
            var result = await _authService.LogoutAsync();
            return HandleResult(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(Authentication_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshToken_RequestDTO request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return HandleResult(result);
        }
    }
}