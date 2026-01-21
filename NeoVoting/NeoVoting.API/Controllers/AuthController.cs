using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;

namespace NeoVoting.API.Controllers
{
    /// <summary>
    /// Handles authentication operations for voters and candidates.
    /// </summary>
    [AllowAnonymous]
    public class AuthController : ApiController
    {
        private readonly IAuthServices _authService;

        public AuthController(IAuthServices authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns access and refresh tokens.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires valid username and password.
        /// - Returns 401 if credentials are invalid.
        /// - Returns 403 if account is locked due to multiple failed attempts.
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(typeof(Authentication_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] Login_RequestDTO request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Registers a new voter account in the system.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires valid National ID and Voting Token from Government System.
        /// - Username must be unique.
        /// - Password and Confirm Password must match.
        /// - Returns 409 if voter is already registered or has already voted.
        /// </remarks>
        [HttpPost("register/voter")]
        [ProducesResponseType(typeof(Registration_ResetPassword_ResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterVoter([FromBody] Register_ResetPassword_VoterOrCandidate_RequestDTO request, CancellationToken ct)
        {
            var result = await _authService.RegisterVoterOrCandidateAsync(request, RoleTypesEnum.Voter, ct);
            return HandleResult(result, Created: true);
        }

        /// <summary>
        /// Registers a new candidate account in the system.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires valid National ID and Nomination Token from Government System.
        /// - Username must be unique.
        /// - Password and Confirm Password must match.
        /// - Returns 409 if candidate is already registered.
        /// </remarks>
        [HttpPost("register/candidate")]
        [ProducesResponseType(typeof(Registration_ResetPassword_ResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterCandidate([FromBody] Register_ResetPassword_VoterOrCandidate_RequestDTO request, CancellationToken ct)
        {
            var result = await _authService.RegisterVoterOrCandidateAsync(request, RoleTypesEnum.Candidate, ct);
            return HandleResult(result, Created: true);
        }

        /// <summary>
        /// Resets the password for a voter or candidate account.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires valid National ID and Token (Voting or Nomination) from Government System.
        /// - Username must match the registered account.
        /// - Password and Confirm Password must match.
        /// - Returns 403 if account type cannot be reset via this portal.
        /// </remarks>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(Registration_ResetPassword_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] Register_ResetPassword_VoterOrCandidate_RequestDTO request, CancellationToken ct)
        {
            var result = await _authService.ResetVoterOrCandidatePasswordAsync(request, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Refreshes the access token using a valid refresh token.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires a valid (non-expired) refresh token.
        /// - Requires the original access token (can be expired).
        /// - Returns 401 if tokens are invalid or expired.
        /// </remarks>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(Authentication_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshToken_RequestDTO request, CancellationToken ct)
        {
            var result = await _authService.RefreshTokenAsync(request, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Logs out the current user by invalidating their refresh token.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires a valid access token (authenticated user).
        /// - Invalidates the refresh token to prevent further token refresh.
        /// - Current access token remains valid until expiry.
        /// </remarks>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var result = await _authService.LogoutAsync(ct);
            return HandleResult(result);
        }
    }
}