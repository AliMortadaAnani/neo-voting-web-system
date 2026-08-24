using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Controllers
{
    public class AuthController : ApiController
    {
        private readonly IAdminServices _adminServices;
        private readonly ICandidateServices _candidateServices;

        public AuthController(IAdminServices adminServices, ICandidateServices candidateServices)
        {
            _adminServices = adminServices;
            _candidateServices = candidateServices;
        }

        /// <summary>
        /// Login to the Government System as an Admin.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Correct Username and Password required
        /// - Requested from allowed IPs only
        /// </remarks>
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

        /// <summary>
        /// Logout from the Government System.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Authentication required
        /// - Requested from allowed IPs only
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var result = await _adminServices.LogoutAsync();
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<IActionResult> Test([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _candidateServices.GetPaginatedCandidatesAsync(pageNumber, pageSize);
            return HandleResult(result);
        }
    }
}