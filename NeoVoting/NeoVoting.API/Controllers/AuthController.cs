using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NeoVoting.Application.RequestDTOs.AuthDTOs;
using NeoVoting.Application.ResponseDTOs.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;

namespace NeoVoting.API.Controllers
{
    [EnableRateLimiting("AuthLimiter")]
    public class AuthController : ApiController
    {
        private readonly IAuthServices _authServices;
        private readonly ILogger<AuthController> _logger;
        private const string RefreshTokenCookieName = "refresh";

        public AuthController(IAuthServices authServices, ILogger<AuthController> logger)
        {
            _authServices = authServices;
            _logger = logger;
        }

        // 1. POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(Authentication_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)] //login failed due to account being locked out
        public async Task<IActionResult> Login([FromBody] Login_RequestDTO loginDTO)
        {
            _logger.LogInformation("Login attempt initiated for user: {UserName}", loginDTO.UserName);

            var result = await _authServices.LoginAsync(loginDTO);

            if (result.IsSuccess)
            {
                _logger.LogInformation("User login successful for: {UserName}", loginDTO.UserName);
                SetRefreshTokenCookie(result.Value.RefreshToken!);
            }
            else
            {
                _logger.LogWarning("User login failed for {UserName}: {Error}", loginDTO.UserName, result.Error.Description);
            }

            return HandleResult(result);
        }

        // 2. POST: api/auth/logout
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout initiated");

            var result = await _authServices.LogoutAsync();

            if (result.IsSuccess)
            {
                _logger.LogInformation("User logout successful");
                Response.Cookies.Delete(RefreshTokenCookieName);
            }
            else
            {
                _logger.LogWarning("User logout failed: {Error}", result.Error.Description);
            }

            return HandleResult(result);
        }

        // 3. POST: api/auth/refresh-token
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(Authentication_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken_RequestDTO? refreshTokenRequestDTO)
        {
            _logger.LogInformation("Refresh token attempt initiated");

            // 1. Extract Access Token from the Authorization Header ("Bearer eyJ...")
            string? accessTokenFromHeader = null;
            if (Request.Headers.TryGetValue("Authorization", out var authHeader) &&
                authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                accessTokenFromHeader = authHeader.ToString()["Bearer ".Length..].Trim();
            }

            // Extract the refresh token from the incoming cookie if available
            string? refreshTokenFromCookie = Request.Cookies[RefreshTokenCookieName];

            var result = await _authServices.RefreshTokenAsync(refreshTokenRequestDTO, refreshTokenFromCookie, accessTokenFromHeader);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Refresh token generated successfully");
                SetRefreshTokenCookie(result.Value.RefreshToken!);
            }
            else
            {
                _logger.LogWarning("Refresh token failed: {Error}", result.Error.Description);
            }

            return HandleResult(result);
        }

        // 4. POST: api/auth/register-voter
        [AllowAnonymous]
        [HttpPost("register-voter")]
        [ProducesResponseType(typeof(RegisterVoterOrCandidate_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterVoter([FromBody] RegisterVoterOrCandidate_RequestDTO dto)
        {
            _logger.LogInformation("Voter registration initiated");

            var result = await _authServices.RegisterVoterOrCandidateAsync(dto, RoleTypesEnum.Voter);

            if (result.IsSuccess)
                _logger.LogInformation("Voter registration successful");
            else
                _logger.LogWarning("Voter registration failed: {Error}", result.Error.Description);

            return HandleResult(result, Created: true);
        }

        // 5. POST: api/auth/register-candidate
        [AllowAnonymous]
        [HttpPost("register-candidate")]
        [ProducesResponseType(typeof(RegisterVoterOrCandidate_ResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterVoterOrCandidate_RequestDTO dto)
        {
            _logger.LogInformation("Candidate registration initiated");

            var result = await _authServices.RegisterVoterOrCandidateAsync(dto, RoleTypesEnum.Candidate);

            if (result.IsSuccess)
                _logger.LogInformation("Candidate registration successful");
            else
                _logger.LogWarning("Candidate registration failed: {Error}", result.Error.Description);

            return HandleResult(result, Created: true);
        }

        // ==========================================
        // PRIVATE HELPERS
        // ==========================================
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Change to false if debugging locally without HTTPS
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(14)
            };

            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
        }
    }
}