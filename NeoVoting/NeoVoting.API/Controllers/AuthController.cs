using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
namespace NeoVoting.API.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
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
    }
}
