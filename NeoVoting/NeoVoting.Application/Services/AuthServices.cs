using Microsoft.AspNetCore.Identity;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenServices _tokenServices;

        public AuthServices(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenServices tokenServices)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenServices = tokenServices;
        }

        public async Task<Result<AuthenticationResponse>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.Username!);
            if (user == null)
            {
                return Result<AuthResponse>.Failure(Error.Unauthorized("Admin.NotValid", "Invalid admin credentials."));
            }
            // This method creates the Set-Cookie header
            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginDTO.Password!,
                isPersistent: true, //better be false for secutity reasons
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Result<AuthResponse>.Success(new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Login successful",
                    Username = user.UserName!,
                    Role = "Admin"
                });
            }

            return Result<AuthResponse>.Failure(Error.Unauthorized("Admin.NotValid", "Invalid admin credentials."));
        }

        public async Task<Result<bool>> LogoutAsync()
        {
            // This tells the browser to delete the cookie
            await _signInManager.SignOutAsync();
            return Result<string>.Success("Logout Successful");
        }

        public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterCandidateAsync(RegisterCandidateDTO registrationDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterAsync(RegisterVoterDTO registrationDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetCandidatePasswordAsync(ResetCandidatePasswordDTO resetPasswordDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterPasswordAsync(ResetVoterPasswordDTO resetPasswordDTO)
        {
            throw new NotImplementedException();
        }
    }
}
