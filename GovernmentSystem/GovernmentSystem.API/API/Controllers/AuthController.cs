using GovernmentSystem.API.API.Filters;
using GovernmentSystem.API.Application.AdminDTOs;
using GovernmentSystem.API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null) return Unauthorized("Invalid Credentials");

            // This method creates the Set-Cookie header
            var result = await _signInManager.PasswordSignInAsync(
                user,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Login Successful. Cookie Set." });
            }

            return Unauthorized("Invalid Credentials");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // This tells the browser to delete the cookie
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logged out" });
        }

        // Test Endpoint
        [HttpGet("check-status")]
        [Authorize] // <--- If Cookie is missing, this returns 401
        public IActionResult CheckStatus()
        {
            var name = User.Identity?.Name ?? "Unknown";
            return Ok($"You are logged in as {name}");
        }

        [HttpGet("test-api-key")]
        [ApiKeyAuth]
        public IActionResult testApiKe()
        {
            return Ok("API Key is valid.");
        }
    }
}
