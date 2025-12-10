using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenServices _tokenServices;
        private readonly IConfiguration _configuration;
        public AuthServices(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenServices tokenServices, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenServices = tokenServices;
            _configuration = configuration;
        }

        public async Task<Result<AuthenticationResponse>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName!);

            if (user == null)
            {
                return Result<AuthenticationResponse>.Failure(Error.Unauthorized("User.NotValid", "Invalid user credentials."));
            }

            // IMPROVEMENT: Use CheckPasswordSignInAsync instead of CheckPasswordAsync
            // This enables "LockoutOnFailure" (the 'true' parameter).
            // It protects against brute force attacks.
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password!, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return Result<AuthenticationResponse>.Failure(Error.Forbidden("User.LockedOut", "Account is locked due to multiple failed attempts. Try again later."));
                }
                return Result<AuthenticationResponse>.Failure(Error.Unauthorized("User.NotValid", "Invalid user credentials."));
            }

            // 1. Generate Tokens
            var authResponse = await _tokenServices.CreateTokensAsync(user);

            // 2. Save Refresh Token to Database
            var refreshDays = double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"]!);
            var refreshExpiry = DateTime.UtcNow.AddDays(refreshDays);

            // Helper method in ApplicationUser entity
            user.UpdateRefreshToken(authResponse.RefreshToken, refreshExpiry);

            await _userManager.UpdateAsync(user);

            return Result<AuthenticationResponse>.Success(authResponse);
        }

        // CHANGED: We need the userId (or Principal) to know WHO to logout.
        // We cannot rely on SignOutAsync because that only kills Cookies.
        public async Task<Result<bool>> LogoutAsync(LogoutDTO logoutDTO ,CancellationToken cancellationToken = default)
        {
            /*//in Controller we will pass the user id in logoutDTO
            // Read the user id from JWT claims
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //            [Authorize]
            //            [HttpPost("logout")]
            //            public async Task<IActionResult> Logout(CancellationToken ct)
            //            {
            //                var claimsUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            //                var dto = new LogoutDTO { Id = Guid.Parse(claimsUserId) }; // not client-supplied
            //                var result = await _authServices.LogoutAsync(dto, ct);
            //                ...
            //}

            //Fluent Validation manual implementation example:
            //        public class AuthServices : IAuthServices
            //    {
            //        private readonly IValidator<LogoutDTO> _logoutValidator;

            //        public AuthServices(..., IValidator<LogoutDTO> logoutValidator)
            //        {
            //            _logoutValidator = logoutValidator;
            //        }

            //        public async Task<Result<bool>> LogoutAsync(LogoutDTO logoutDTO, CancellationToken cancellationToken = default)
            //        {
            //            var validationResult = await _logoutValidator.ValidateAsync(logoutDTO, cancellationToken);
            //            if (!validationResult.IsValid)
            //            {
            //                return Result<bool>.Failure(
            //                    Error.Validation("Logout.Invalid", validationResult.ToString()));
            //            }

            //            var user = await _userManager.FindByIdAsync(logoutDTO.Id.ToString());
            //            ...
            //}
            //    }*/

            if (logoutDTO.Id == Guid.Empty)
            {
                return Result<bool>.Failure(Error.Validation("User.IdMissing", "User ID is required for logout."));
            }

            var user = await _userManager.FindByIdAsync(logoutDTO.Id.ToString()!);

            if (user == null)
            {
                // Technically success because if they don't exist, they are logged out.
                return Result<bool>.Success(true);
            }

            // 1. Invalidate the Refresh Token in the DB
            // This prevents them from ever generating a new Access Token.
            // Their current Access Token will remain valid until it expires naturally (e.g., 15 mins).
            user.InvalidateRefreshToken();

            // 2. Save changes
            await _userManager.UpdateAsync(user);

            // 3. Optional: Still call SignOut in case you mix Cookies/JWT
            await _signInManager.SignOutAsync();

            return Result<bool>.Success(true);
        }
      

        public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken)
        {
            // 1.Extract claims from the EXPIRED access token
         var principal = _tokenServices.GetPrincipalFromExpiredToken(refreshTokenRequestDTO.AccessToken);

            if (principal == null) return Result<AuthenticationResponse>.Failure(Error.Validation("Token.NotValid", "Invalid token")); // Invalid token format

            // 2. Get the User ID from the claims
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null || user.RefreshToken != refreshTokenRequestDTO.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
            {
                return Result<AuthenticationResponse>.Failure(Error.Validation("Token.NotValid", "Invalid token")); // Invalid or Expired Refresh Token
            }

            // 3. Generate NEW tokens
            var newAuthResponse = await _tokenServices.CreateTokensAsync(user);

            var refreshExpiry = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"]!));

            // 4. Update the DB with the NEW refresh token (Rotate them for security)
            user.UpdateRefreshToken(newAuthResponse.RefreshToken, refreshExpiry);
            await _userManager.UpdateAsync(user);

            return Result<AuthenticationResponse>.Success(newAuthResponse);
        }

        // =========================================================================================
        // REGISTRATION & RESET PASSWORD IMPLEMENTATION
        // =========================================================================================

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterAsync(RegisterVoterDTO registrationDTO, CancellationToken cancellationToken = default)
        {
            // 1. Retrieve the Voter from the Civil Registry (Domain Table)
            var voter = await _dbContext.Voters
                .FirstOrDefaultAsync(v => v.NationalId == registrationDTO.NationalId, cancellationToken);

            // 2. Validations
            if (voter == null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("Voter.NotFound", "National ID not found in voter registry."));

            if (voter.IsRegistered)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Conflict("Voter.AlreadyRegistered", "This voter is already registered."));

            if (voter.VotingToken != registrationDTO.VotingToken)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Forbidden("Voter.InvalidToken", "Invalid Voting Token."));

            // 3. Create Identity User (ApplicationUser)
            // Use NationalId as UserName
            string userName = registrationDTO.NationalId.ToString()!;

            ApplicationUser appUser;
            try
            {
                appUser = ApplicationUser.CreateVoterOrCandidateAccount(
                    userName,
                    voter.FirstName,
                    voter.LastName,
                    voter.DateOfBirth,
                    voter.Gender,
                    voter.GovernorateId
                );
            }
            catch (ArgumentException ex)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Voter.DataInvalid", ex.Message));
            }

            // 4. Save Identity User (No password yet - user must call ResetPassword or we could generate a default)
            // Note: Since RegisterVoterDTO has no password, we create it without one. 
            // The user won't be able to login until they set a password (via Reset flow) or we'd need to change DTO.
            var createResult = await _userManager.CreateAsync(appUser);
            if (!createResult.Succeeded)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.CreationFailed", createResult.Errors.First().Description));
            }

            // 5. Assign Role
            var roleResult = await _userManager.AddToRoleAsync(appUser, RoleTypesEnum.Voter.ToString());
            if (!roleResult.Succeeded)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.RoleFailed", "Failed to assign voter role."));
            }

            // 6. Mark Voter as Registered in Domain Table
            voter.IsRegistered = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 7. Return Result
            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(appUser, RoleTypesEnum.Voter.ToString()));
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterCandidateAsync(RegisterCandidateDTO registrationDTO, CancellationToken cancellationToken = default)
        {
            // 1. Retrieve Candidate
            var candidate = await _dbContext.Candidates
                .FirstOrDefaultAsync(c => c.NationalId == registrationDTO.NationalId, cancellationToken);

            // 2. Validations
            if (candidate == null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("Candidate.NotFound", "National ID not found in candidate registry."));

            if (candidate.IsRegistered)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Conflict("Candidate.AlreadyRegistered", "This candidate is already registered."));

            if (candidate.NominationToken != registrationDTO.NominationToken)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Forbidden("Candidate.InvalidToken", "Invalid Nomination Token."));

            // 3. Create Identity User
            string userName = registrationDTO.NationalId.ToString()!;
            ApplicationUser appUser;
            try
            {
                appUser = ApplicationUser.CreateVoterOrCandidateAccount(
                    userName,
                    candidate.FirstName,
                    candidate.LastName,
                    candidate.DateOfBirth,
                    candidate.Gender,
                    candidate.GovernorateId
                );
            }
            catch (ArgumentException ex)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Candidate.DataInvalid", ex.Message));
            }

            // 4. Save Identity User
            var createResult = await _userManager.CreateAsync(appUser);
            if (!createResult.Succeeded)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.CreationFailed", createResult.Errors.First().Description));
            }

            // 5. Assign Role
            var roleResult = await _userManager.AddToRoleAsync(appUser, RoleTypesEnum.Candidate.ToString());
            if (!roleResult.Succeeded)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.RoleFailed", "Failed to assign candidate role."));
            }

            // 6. Mark Candidate as Registered
            candidate.IsRegistered = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(appUser, RoleTypesEnum.Candidate.ToString()));
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterPasswordAsync(ResetVoterPasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        {
            // 1. Validate Password Match
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));

            // 2. Validate against Voter Registry (Security Check: Must have valid Token)
            var voter = await _dbContext.Voters
                .FirstOrDefaultAsync(v => v.NationalId == resetPasswordDTO.NationalId, cancellationToken);

            if (voter == null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("Voter.NotFound", "Voter not found."));

            if (voter.VotingToken != resetPasswordDTO.VotingToken)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Forbidden("Voter.InvalidToken", "Invalid Voting Token. Cannot reset password."));

            // 3. Find Identity User
            var user = await _userManager.FindByNameAsync(resetPasswordDTO.UserName!);
            if (user == null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("User.NotFound", "User account not found."));

            // 4. Force Reset Password (Remove old, Add new)
            // Since we validated the VotingToken, we trust this request.
            if (await _userManager.HasPasswordAsync(user))
            {
                await _userManager.RemovePasswordAsync(user);
            }

            var addPassResult = await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword!);

            if (!addPassResult.Succeeded)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.ChangeFailed", addPassResult.Errors.First().Description));
            }

            // 5. Important: Update Security Stamp to invalidate old cookies/tokens
            await _userManager.UpdateSecurityStampAsync(user);

            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(user, RoleTypesEnum.Voter.ToString()));
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetCandidatePasswordAsync(ResetCandidatePasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        {
            // 1. Validate Password Match
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));

            // 2. Validate against Candidate Registry
            var candidate = await _dbContext.Candidates
                .FirstOrDefaultAsync(c => c.NationalId == resetPasswordDTO.NationalId, cancellationToken);

            if (candidate == null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("Candidate.NotFound", "Candidate not found."));

            if (candidate.NominationToken != resetPasswordDTO.NominationToken)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Forbidden("Candidate.InvalidToken", "Invalid Nomination Token. Cannot reset password."));

            // 3. Find Identity User
            var user = await _userManager.FindByNameAsync(resetPasswordDTO.UserName!);
            if (user == null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("User.NotFound", "User account not found."));

            // 4. Force Reset Password
            if (await _userManager.HasPasswordAsync(user))
            {
                await _userManager.RemovePasswordAsync(user);
            }

            var addPassResult = await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword!);

            if (!addPassResult.Succeeded)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.ChangeFailed", addPassResult.Errors.First().Description));
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(user, RoleTypesEnum.Candidate.ToString()));
        }

        // --- Helper Method ---
        private static Registration_ResetPassword_ResponseDTO MapToResponseDTO(ApplicationUser user, string role)
        {
            return new Registration_ResetPassword_ResponseDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                GovernorateId = user.GovernorateID,
                Role = role
            };
        }
    }
}