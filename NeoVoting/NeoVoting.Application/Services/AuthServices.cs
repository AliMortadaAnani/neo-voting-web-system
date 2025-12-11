using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.NeoVotingResponsesDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenServices _tokenServices;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthServices> _logger;
        private readonly RoleManager<ApplicationRole> roleManager;
        public AuthServices(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenServices tokenServices, IConfiguration configuration, IHttpClientFactory httpClientFactory,ILogger<AuthServices> logger, RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenServices = tokenServices;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            this.roleManager = roleManager;
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
            //var refreshDays = double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"]!);
            //var refreshExpiry = DateTime.UtcNow.AddDays(refreshDays);

            // Helper method in ApplicationUser entity
            user.UpdateRefreshToken(authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

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
      

        public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken = default)
        {
            // 1.Extract claims from the EXPIRED access token
         var principal = _tokenServices.GetPrincipalFromExpiredToken(refreshTokenRequestDTO.AccessToken);

            if (principal == null) return Result<AuthenticationResponse>.Failure(Error.Validation("Token.NotValid", "Invalid token, please login")); // Invalid token format

            // 2. Get the User ID from the claims
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null || user.RefreshToken != refreshTokenRequestDTO.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
            {
                return Result<AuthenticationResponse>.Failure(Error.Validation("Token.NotValid", "Invalid token, please login")); // Invalid or Expired Refresh Token
            }

            // 3. Generate NEW tokens
            var newAuthResponse = await _tokenServices.CreateTokensAsync(user);

            

            // 4. Update the DB with the NEW refresh token (Rotate them for security)
            user.UpdateRefreshToken(newAuthResponse.RefreshToken, newAuthResponse.RefreshTokenExpiration);
            await _userManager.UpdateAsync(user);

            return Result<AuthenticationResponse>.Success(newAuthResponse);
        }

        // =========================================================================================
        // REGISTRATION & RESET PASSWORD IMPLEMENTATION
        // =========================================================================================
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        private static string Truncate(string? value, int maxLength = 500)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterAsync(RegisterVoterDTO registrationDTO, CancellationToken cancellationToken = default)
        {

            // 1. Validate Passwords Match
            if (registrationDTO.NewPassword != registrationDTO.ConfirmPassword)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));
            }

            // 2. Check for Duplicate Username
            var existingUser = await _userManager.FindByNameAsync(registrationDTO.UserName!);
            if (existingUser != null)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Conflict("User.Exists", "This username is already taken."));
            }

            
          
            if (await roleManager.FindByNameAsync(RoleTypesEnum.Voter.ToString()) is null)
            {
                var applicationRole = ApplicationRole.CreateVoterRole();
                await roleManager.CreateAsync(applicationRole);
            }


            
                var client = _httpClientFactory.CreateClient();

                // You can move this URL to configuration
                var baseUrl = _configuration["NeoVoting:BaseUrl"] ?? "https://localhost:5000";
                var url = $"{baseUrl.TrimEnd('/')}/api/external/voters/verify";

                HttpResponseMessage response;

                try
                {
                // Create body with two GUIDs
                var body = new
                {
                    nationalId  = registrationDTO.NationalId, // replace with your actual value
                    votingToken = registrationDTO.VotingToken  // replace with your actual value
                };
                // Send request as JSON body
                response = await client.PostAsJsonAsync(url, body, _jsonOptions, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while calling GovernmentSystem VerifyVoter endpoint.");
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
                }

                var statusCode = (int)response.StatusCode;
                string content = string.Empty;

                try
                {
                    content = await response.Content.ReadAsStringAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to read response content from GovernmentSystem VerifyVoter.");
                // If we can’t read content, still return a generic failure
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
            }

                // Handle 2xx OK
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var voter = JsonSerializer.Deserialize<NeoVoting_VoterResponseDTO>(content, _jsonOptions);

                        if (voter == null)
                        {
                            _logger.LogWarning("Government VerifyVoter returned 200 but body was null or invalid.");
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
                        }

                    DateTime dateOfBirthDateTime = voter.DateOfBirth.ToDateTime(TimeOnly.MinValue);
                    int governorateIdInt = (int)voter.GovernorateId;

                    var registeredVoter = ApplicationUser.CreateVoterOrCandidateAccount(
                            registrationDTO.UserName!,
                            voter.FirstName,
                            voter.LastName,
                            dateOfBirthDateTime,
                            voter.Gender,
                            governorateIdInt
                            );

                    var createResult = await _userManager.CreateAsync(registeredVoter, registrationDTO.NewPassword!);
                    if (!createResult.Succeeded)
                    {
                        var errorMsg = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.CreationFailed", errorMsg));
                    }
                    
                    if (createResult.Succeeded)
                    {
                   
                    // 6. Assign "Voter" Role
                    var roleResult = await _userManager.AddToRoleAsync(registeredVoter, RoleTypesEnum.Voter.ToString());

                    if (!roleResult.Succeeded)
                    {
                        // Optional: Cleanup user if role assignment fails
                        await _userManager.DeleteAsync(registeredVoter);
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.RoleFailed", "Failed to assign voter role."));
                    }

                    var registerResponseDto = new Registration_ResetPassword_ResponseDTO
                    {
                        Id = registeredVoter.Id,
                        UserName = registeredVoter.UserName,
                        FirstName = registeredVoter.FirstName,
                        LastName = registeredVoter.LastName,
                        DateOfBirth = registeredVoter.DateOfBirth,
                        GovernorateId = registeredVoter.GovernorateID,
                        Gender = registeredVoter.Gender,
                        Role =  RoleTypesEnum.Voter.ToString() 
                        };

                    return Result<Registration_ResetPassword_ResponseDTO>.Success(registerResponseDto);
                    }
                    catch (JsonException jex)
                    {
                        _logger.LogError(jex,
                            "Failed to deserialize GovernmentSystem VerifyVoter successful response to NeoVoting_VoterResponseDTO. Content: {Content}",
                            Truncate(content));
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
                }
                }

                // Non-success: 400, 401, 404, 500, etc.
                // Try to interpret as ProblemDetails / ValidationProblemDetails
                try
                {
                    // We first try ValidationProblemDetails for 400/401,
                    // but ProblemDetails is enough for uniform handling.
                    var problem = JsonSerializer.Deserialize<ProblemDetails>(content, _jsonOptions);

                    if (problem != null)
                    {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
                }
                }
                catch (JsonException jex)
                {
                    _logger.LogWarning(jex,
                        "Failed to deserialize NeoVoting error response as ProblemDetails. Status: {StatusCode}, Content: {Content}",
                        statusCode, Truncate(content));

                }

            return Result<Registration_ResetPassword_ResponseDTO>.Failure(
Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
        }
        

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterPasswordAsync(ResetVoterPasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        {
            // 1. Validate Passwords Match
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));
            }

            // 2. Find User
            var user = await _userManager.FindByNameAsync(resetPasswordDTO.UserName!);

            if (user == null)
            {
                // Security Best Practice: Usually returns vague error, but for this context "Not Found" is acceptable
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("User.NotFound", "User account not found."));
            }

            // 3. Validate National ID match (Optional Security Check)
            // If the DTO sends a NationalId, verify it matches the User's ID
            if (resetPasswordDTO.NationalId.HasValue && user.Id != resetPasswordDTO.NationalId.Value)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.Mismatch", "National ID does not match the username provided."));
            }

            /* 
             * NOTE ON VOTING TOKEN:
             * The DTO contains 'VotingToken', but there is no 'Voters' table to verify it against,
             * and 'ApplicationUser' does not store it. 
             * Therefore, we cannot perform a token validation here in this isolated Identity context.
             * We proceed assuming the caller has been verified by other means or if we ignore the token.
             */

            // 4. Force Reset Password (Remove Old -> Add New)
            // This is an administrative reset/forgot password flow
            if (await _userManager.HasPasswordAsync(user))
            {
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.RemoveFailed", "Failed to process password reset."));
                }
            }

            var addResult = await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword!);

            if (!addResult.Succeeded)
            {
                var errorMsg = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.ChangeFailed", errorMsg));
            }

            // 5. Invalidate existing sessions (Security Stamp)
            // This ensures any old cookies or tokens (if checking stamp) are invalidated
            await _userManager.UpdateSecurityStampAsync(user);

            // Also invalidate refresh token to force re-login
            user.InvalidateRefreshToken();
            await _userManager.UpdateAsync(user);

            // 6. Return Response
            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(user, RoleTypesEnum.Voter.ToString()));
        }

        // --- Helper Method ---
        private static Registration_ResetPassword_ResponseDTO MapToResponseDTO(ApplicationUser user, string role)
        {
            return new Registration_ResetPassword_ResponseDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                // These will likely be null since RegisterVoterDTO didn't provide them
                // and we used the Admin factory.
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                GovernorateId = user.GovernorateID,
                Role = role
            };
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