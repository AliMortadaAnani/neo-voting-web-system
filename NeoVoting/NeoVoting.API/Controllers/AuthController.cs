using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NeoVoting.Application.RequestDTOs.AuthDTOs;
using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;

namespace NeoVoting.API.Controllers
{
    [EnableRateLimiting("AuthLimiter")]
    public class AuthController : ApiController
    {
        private readonly IAuthServices _authServices;
        private readonly IFileServices _fileServices;
        private readonly ILogger<AuthController> _logger;
        private const string RefreshTokenCookieName = "refresh";

        public AuthController(IAuthServices authServices, ILogger<AuthController> logger, IFileServices fileServices)
        {
            _authServices = authServices;
            _logger = logger;
            _fileServices = fileServices;
        }

        // 1. POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(Authentication_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] Login_RequestDTO loginDTO)
        {
            _logger.LogInformation("Login attempt initiated for user: {UserName}", loginDTO.UserName);

            var result = await _authServices.LoginAsync(loginDTO);

            if (result.IsSuccess)
            {
                _logger.LogInformation("User login successful for: {UserName}", loginDTO.UserName);
                SetRefreshTokenCookie(result.Value.RefreshToken);
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

            var result = await _authServices.RefreshTokenAsync(refreshTokenRequestDTO, refreshTokenFromCookie,accessTokenFromHeader);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Refresh token generated successfully");
                SetRefreshTokenCookie(result.Value.RefreshToken);
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
        // ROLE TESTING ENDPOINTS (3 endpoints)
        // ==========================================

        // 6. GET: api/auth/test-admin
        [Authorize(Roles = "Admin")]
        [HttpGet("test-admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public IActionResult TestAdminRole()
        {
            _logger.LogInformation("Admin test endpoint accessed successfully");
            return Ok(new { message = "Success! You have access as an Admin." });
        }

        // 7. GET: api/auth/test-candidate
        [Authorize(Roles = "Candidate")]
        [HttpGet("test-candidate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public IActionResult TestCandidateRole()
        {
            _logger.LogInformation("Candidate test endpoint accessed successfully");
            return Ok(new { message = "Success! You have access as a Candidate." });
        }

        // 8. GET: api/auth/test-voter
        [Authorize(Roles = "Voter")]
        [HttpGet("test-voter")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public IActionResult TestVoterRole()
        {
            _logger.LogInformation("Voter test endpoint accessed successfully");
            return Ok(new { message = "Success! You have access as a Voter." });
        }


        // POST: api/candidate/profile-photo
        [HttpPost("profile-photo")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadProfilePhoto([FromForm] CandidateProfileUploadImage_RequestDTO requestDTO)
        {
            _logger.LogInformation("Profile photo upload initiated");

            var result = await _fileServices.SaveFileAsync(requestDTO);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Profile photo uploaded successfully: {Path}", result.Value);
            }
            else
            {
                _logger.LogWarning("Profile photo upload failed: {Error}", result.Error.Description);
            }

            return HandleResult(result);
        }

        // DELETE: api/candidate/profile-photo
        [HttpDelete("profile-photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public IActionResult DeleteProfilePhoto([FromQuery] string fileUrl)
        {
            _logger.LogInformation("Profile photo deletion initiated for: {Url}", fileUrl);

            // Note: If DeleteFile is void/synchronous based on your interface, 
            // you can wrap or call it directly.
            _fileServices.DeleteFile(fileUrl);

            _logger.LogInformation("Profile photo deletion processed");
            return Ok(new { message = "File deleted successfully." });
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