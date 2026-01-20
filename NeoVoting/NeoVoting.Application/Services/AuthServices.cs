using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Contracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;
using NeoVoting.Domain.IdentityEntities;
using NeoVoting.Domain.RepositoryContracts;
using System.Security.Claims;

namespace NeoVoting.Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IGovernmentSystemGateway _govGateway;
        private readonly ITokenServices _tokenServices;
        private readonly ISystemAuditLogRepository _systemAuditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuthServices> _logger;
        
        
        

        public AuthServices
            (SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenServices tokenServices, ILogger<AuthServices> logger, RoleManager<ApplicationRole> roleManager, ISystemAuditLogRepository systemAuditLogRepository, IUnitOfWork unitOfWork, IGovernmentSystemGateway governmentSystemGateway,ICurrentUserService currentUserService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenServices = tokenServices;
            _govGateway = governmentSystemGateway;
            _logger = logger;
            _roleManager = roleManager;
            _systemAuditLogRepository = systemAuditLogRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }


        public async Task<Result<Authentication_ResponseDTO>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName!);

            if (user == null)
            {
                return Result<Authentication_ResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.User_InvalidCredentials),
                    "Invalid user credentials."));
            }

            // IMPROVEMENT: Use CheckPasswordSignInAsync instead of CheckPasswordAsync
            // This enables "LockoutOnFailure" (the 'true' parameter).
            // It protects against brute force attacks.
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password!, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return Result<Authentication_ResponseDTO>.Failure(Error.Forbidden
                        (nameof(ProblemDetails403ErrorTypes.User_LockedOut),
                        "Account is locked due to multiple failed attempts. Try again later."));
                }
                return Result<Authentication_ResponseDTO>.Failure
                    (Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.User_InvalidCredentials),
                    "Invalid user credentials."));
            }

            // 1. Generate Tokens
            var authResponse = await _tokenServices.CreateTokensAsync(user);

            
            // Helper method in ApplicationUser entity
            user.UpdateRefreshToken(authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Login_Failed), $"Failed to update refresh token: {errors}"));
            }

            return Result<Authentication_ResponseDTO>.Success(authResponse);
        }

        //[Authorize] (only authenticated users should call this unlike other methods Here)
        public async Task<Result<bool>> LogoutAsync(CancellationToken cancellationToken = default)
        {
            // 1. Get authenticated user ID (throws if not authenticated)
            // This is safe because the endpoint requires [Authorize]
            // if it throws here, something is very wrong !!!!
            var userId = _currentUserService.GetAuthenticatedUserId();




            // 2. Find user in database
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                // User doesn't exist in DB - they're effectively logged out
                // This is a success case (idempotent logout)
                return Result<bool>.Success(true);
            }

            // 3. Invalidate the refresh token
            // This prevents them from generating new access tokens
            // Current access token remains valid until expiry
            user.InvalidateRefreshToken();

            // 4. Save changes
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result<bool>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Logout_Failed), $"Failed to invalidate refresh token: {errors}"));
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<Authentication_ResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken = default)
        {
            // 1. Validate the old Access Token
            var principalResult = _tokenServices.GetPrincipalFromExpiredToken(refreshTokenRequestDTO.AccessToken);

            // 2. CHECK FAILURE (Clean check, no try/catch needed)
            if (principalResult.IsFailure)
            {
                // Pass the specific error up (e.g. "Token.Invalid")
                return Result<Authentication_ResponseDTO>.Failure(principalResult.Error);
            }

            // 3. Extract User ID
            var principal = principalResult.Value; // Safe access
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            // SAFETY CHECK
            if (string.IsNullOrEmpty(userId))
            {
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken), "Token is missing user identity."));
            }

            // 2. Validate the Refresh Token (from DB)
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null || user.RefreshToken != refreshTokenRequestDTO.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
            {
                return Result<Authentication_ResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken), "Invalid token, please login")); // Invalid or Expired Refresh Token
            }

            // 3. Generate NEW tokens
            var newAuthResponse = await _tokenServices.CreateTokensAsync(user);

            // 4. Update the DB with the NEW refresh token (Rotate them for security)
            user.UpdateRefreshToken(newAuthResponse.RefreshToken, newAuthResponse.RefreshTokenExpiration);
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.RefreshToken_Failed), $"Failed to invalidate refresh token: {errors}"));
            }
            return Result<Authentication_ResponseDTO>.Success(newAuthResponse);
        }

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
                GovernorateId = user.GovernorateId,
                Role = role
            };
        }



        public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterOrCandidateAsync(
    Register_ResetPassword_VoterOrCandidate_DTO dto,
    RoleTypesEnum role,
    CancellationToken ct = default)
        {
            // -----------------------------------------------------------------------
            // PHASE 1: PRE-FLIGHT CHECKS (Fast & Cheap)
            // -----------------------------------------------------------------------
            // We check these first because they don't cost money (no external API calls)
            // and don't modify the database.

            if (dto.NewPassword != dto.ConfirmPassword)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Password_Mismatch), "Passwords do not match."));

            if (await _userManager.FindByNameAsync(dto.UserName!) != null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Conflict(nameof(ProblemDetails409ErrorTypes.User_DuplicateUsername), "This username is already taken."));

            // Ensure "Voter" Role Exists
            if (await _roleManager.FindByNameAsync(RoleTypesEnum.Voter.ToString()) is null)
            {
                ApplicationRole voterRole = ApplicationRole.CreateVoterRole();


                var roleResult = await _roleManager.CreateAsync(voterRole);
                if (!roleResult.Succeeded)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.Role_CreationFailed), roleResult.Errors.First().Description));
                }
            }

            // Ensure "Candidate" Role Exists
            if (await _roleManager.FindByNameAsync(RoleTypesEnum.Candidate.ToString()) is null)
            {
                ApplicationRole candidateRole = ApplicationRole.CreateCandidateRole();

                var roleResult = await _roleManager.CreateAsync(candidateRole);
                if (!roleResult.Succeeded)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.Role_CreationFailed), roleResult.Errors.First().Description));
                }
            }

            if (role == RoleTypesEnum.Voter)
            {
                // -----------------------------------------------------------------------
                // PHASE 2: GET THE TRUTH (The Gateway Call)
                // -----------------------------------------------------------------------
                // We rely on the Government System as the "Source of Truth" for user details.

                var verifyRequest = new NeoVoting_GetVoterRequestDTO
                {
                    NationalId = dto.NationalId,
                    VotingToken = dto.VotingOrNominationToken
                };

                // CALL GATEWAY: This handles the HTTP Post, Try/Catch, and JSON Parsing.
                var verifyResult = await _govGateway.GetVoterAsync(verifyRequest, ct);

                // If the network failed, or the API returned 404/400/500, we stop here.
                if (verifyResult.IsFailure)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(verifyResult.Error);
                }

                var govData = verifyResult.Value; // This contains Name, DOB, Eligibility, etc.

                // -----------------------------------------------------------------------
                // PHASE 3: APPLY BUSINESS RULES
                // -----------------------------------------------------------------------
                // The API call succeeded, but does the data satisfy our rules?

                //Already checked in Gov System during GetVoter
                /*if (!govData.EligibleForElection)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Voter.Ineligible", "Voter is not eligible for election."));

                if (!govData.ValidToken)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Voter.InvalidToken", "Voting token is invalid."));*/

                if (govData.IsRegistered)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyRegistered), "Voter account already exists."));

                if (govData.Voted)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyVoted),
                        "Voter has already cast a vote."));

                // -----------------------------------------------------------------------
                // PHASE 4: LOCAL COMMIT (Create the Account)
                // -----------------------------------------------------------------------
                // We instantiate the user using data from 'govData' (Verified), NOT 'dto' (User input).
                // This prevents a user from registering as "Batman" when their ID says "Bruce Wayne".

                ApplicationUser newUser = ApplicationUser.CreateVoterOrCandidateAccount(
                   dto.UserName!,
                   govData.FirstName,
                   govData.LastName,
                   govData.DateOfBirth.ToDateTime(TimeOnly.MinValue), // DateOnly -> DateTime
                   govData.Gender,
                   (int)govData.GovernorateId
               );


                // SAVE TO DB
                var createResult = await _userManager.CreateAsync(newUser, dto.NewPassword!);

                if (!createResult.Succeeded)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.UserCreation_Failed), createResult.Errors.First().Description));

                // await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());
                var assignVoterRoleResult = await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());

                if (!assignVoterRoleResult.Succeeded)
                {
                    // Optional: Cleanup user if role assignment fails
                    await _userManager.DeleteAsync(newUser);
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.UserRoleAssignment_Failed),
                        "Failed to assign voter role."));
                }

                // -----------------------------------------------------------------------
                // PHASE 5: DISTRIBUTED COMMIT (The Sync)
                // -----------------------------------------------------------------------
                // We successfully created the local account. Now we MUST tell the Government.

                var confirmRequest = new NeoVoting_VoterIsRegisteredRequestDTO
                {
                    NationalId = dto.NationalId,
                    VotingToken = dto.VotingOrNominationToken,
                    RegisteredUsername = dto.UserName
                };

                var confirmResult = await _govGateway.MarkVoterAsRegisteredAsync(confirmRequest, ct);

                // -----------------------------------------------------------------------
                // PHASE 6: THE COMPENSATING TRANSACTION (The Rollback)
                // -----------------------------------------------------------------------
                // CRITICAL: If the Government says "No" (or crashes) at this specific moment,
                // we have a local user but the Government thinks they aren't registered.
                // We must DELETE the local user to restore consistency.

                if (confirmResult.IsFailure || !confirmResult.Value.IsRegistered)
                {
                    var logError = SystemAuditLog.Create(
                    newUser.Id,
                    newUser.UserName!,
                    SystemActionTypesEnum.ERROR_VOTER_NOT_REGISTERED,
                    $"Voter '{newUser.UserName}' registration failed.",
                    null,
                    null,
                    null
                );


                    var addedErrorLog = await _systemAuditLogRepository.AddSystemAuditLogAsync(logError, ct);
                    if (addedErrorLog == null)
                    {
                        _logger.LogError("Failed to add system audit log for voter {VoterId}", newUser.Id);
                    }

                    var rowsErrorAdded = await _unitOfWork.SaveChangesAsync(ct);
                    if (rowsErrorAdded == 0)
                    {
                        _logger.LogError("SaveChangesAsync returned 0 after adding audit log for voter {VoterId}", newUser.Id);
                    }

                    try
                    {
                        // UNDO Phase 4
                        await _userManager.DeleteAsync(newUser);
                    }
                    catch (Exception ex)
                    {
                        // ADDED: Critical logging for "Zombie User" scenario
                        _logger.LogCritical(ex, "CRITICAL: Failed to rollback user creation for NationalID {NationalId}. User exists in NeoVoting but not valid in Gov System.", dto.NationalId);
                        // We still return failure to the client, but the Admin needs to see this log.
                    }

                    var errorToReturn = confirmResult.IsFailure
                        ? confirmResult.Error
                        : Error.Failure(nameof(ProblemDetails500ErrorTypes.GovernmentSystemGateway_Error),
                        "Government System failed to confirm registration.");



                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(errorToReturn);
                }

                // -----------------------------------------------------------------------
                // PHASE 7: SUCCESS & AUDIT
                // -----------------------------------------------------------------------
                // If we get here:
                // 1. User is in Identity DB.
                // 2. User is marked Registered in Gov DB.
                // Everything is consistent.

                var log = SystemAuditLog.Create(
                    newUser.Id,
                    newUser.UserName!,
                    SystemActionTypesEnum.VOTER_REGISTERED,
                    $"Voter '{newUser.UserName}' registered successfully.",
                    null,
                    null,
                    null
                );


                var addedLog = await _systemAuditLogRepository.AddSystemAuditLogAsync(log, ct);
                if (addedLog == null)
                {
                    _logger.LogError("Failed to add system audit log for voter {VoterId}", newUser.Id);
                }

                var rowsAdded = await _unitOfWork.SaveChangesAsync(ct);
                if (rowsAdded == 0)
                {
                    _logger.LogError("SaveChangesAsync returned 0 after adding audit log for voter {VoterId}", newUser.Id);
                }



                return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(newUser, RoleTypesEnum.Voter.ToString()));
            }
            else
                if (role == RoleTypesEnum.Candidate)
            {

                // -----------------------------------------------------------------------
                // PHASE 2: GET THE TRUTH (The Gateway Call)
                // -----------------------------------------------------------------------

                var verifyRequest = new NeoVoting_GetCandidateRequestDTO
                {
                    NationalId = dto.NationalId,
                    NominationToken = dto.VotingOrNominationToken
                };

                var verifyResult = await _govGateway.GetCandidateAsync(verifyRequest, ct);

                if (verifyResult.IsFailure)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(verifyResult.Error);
                }

                var govData = verifyResult.Value;

                // -----------------------------------------------------------------------
                // PHASE 3: APPLY BUSINESS RULES
                // -----------------------------------------------------------------------


                if (govData.IsRegistered)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Conflict(nameof(ProblemDetails409ErrorTypes.Candidate_AlreadyRegistered),
                        "Candidate account already exists."));

                // -----------------------------------------------------------------------
                // PHASE 4: LOCAL COMMIT (Create the Account)
                // -----------------------------------------------------------------------

                ApplicationUser newUser = ApplicationUser.CreateVoterOrCandidateAccount(
                        dto.UserName!,
                        govData.FirstName,
                        govData.LastName,
                        govData.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                        govData.Gender,
                        (int)govData.GovernorateId
                    );



                var createResult = await _userManager.CreateAsync(newUser, dto.NewPassword!);

                if (!createResult.Succeeded)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.UserCreation_Failed), createResult.Errors.First().Description));

                var assignRoleResult = await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Candidate.ToString());

                if (!assignRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(newUser);
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.UserRoleAssignment_Failed),
                        "Failed to assign candidate role."));
                }

                // -----------------------------------------------------------------------
                // PHASE 5: DISTRIBUTED COMMIT (The Sync)
                // -----------------------------------------------------------------------

                var confirmRequest = new NeoVoting_CandidateIsRegisteredRequestDTO
                {
                    NationalId = dto.NationalId,
                    NominationToken = dto.VotingOrNominationToken,
                    RegisteredUsername = dto.UserName
                };

                var confirmResult = await _govGateway.MarkCandidateAsRegisteredAsync(confirmRequest, ct);

                // -----------------------------------------------------------------------
                // PHASE 6: THE COMPENSATING TRANSACTION (The Rollback)
                // -----------------------------------------------------------------------

                if (confirmResult.IsFailure || !confirmResult.Value.IsRegistered)
                {
                    var logError = SystemAuditLog.Create(
                    newUser.Id,
                    newUser.UserName!,
                    SystemActionTypesEnum.ERROR_CANDIDATE_NOT_REGISTERED,
                    $"Candidate '{newUser.UserName}' registration failed.",
                    null,
                    null,
                    null
                );

                    var addedlogError = await _systemAuditLogRepository.AddSystemAuditLogAsync(logError, ct);
                    if (addedlogError == null)
                    {
                        _logger.LogError("Failed to add system audit log for candidate {CandidateId}", newUser.Id);
                    }

                    int rowAddedError = await _unitOfWork.SaveChangesAsync(ct);
                    if (rowAddedError == 0)
                    {
                        _logger.LogError("SaveChangesAsync returned 0 after adding audit log for candidate {CandidateId}", newUser.Id);
                    }

                    try
                    {
                        await _userManager.DeleteAsync(newUser);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "CRITICAL: Failed to rollback user creation for NationalID {NationalId}. User exists in NeoVoting but not in Gov System.", dto.NationalId);
                    }

                    var errorToReturn = confirmResult.IsFailure
                        ? confirmResult.Error
                        : Error.Failure(nameof(ProblemDetails500ErrorTypes.GovernmentSystemGateway_Error),
                        "Government System failed to confirm registration.");

                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(errorToReturn);
                }

                // -----------------------------------------------------------------------
                // PHASE 7: SUCCESS & AUDIT
                // -----------------------------------------------------------------------

                var log = SystemAuditLog.Create(
                    newUser.Id,
                    newUser.UserName!,
                    SystemActionTypesEnum.CANDIDATE_REGISTERED,
                    $"Candidate '{newUser.UserName}' registered successfully.",
                    null,
                    null,
                    null
                );


                var addedlog = await _systemAuditLogRepository.AddSystemAuditLogAsync(log, ct);
                if (addedlog == null)
                {
                    _logger.LogError("Failed to add system audit log for candidate {CandidateId}", newUser.Id);
                }

                int rowAdded = await _unitOfWork.SaveChangesAsync(ct);
                if (rowAdded == 0)
                {
                    _logger.LogError("SaveChangesAsync returned 0 after adding audit log for candidate {CandidateId}", newUser.Id);
                }


                return Result<Registration_ResetPassword_ResponseDTO>.Success(
                    MapToResponseDTO(newUser, RoleTypesEnum.Candidate.ToString()));
            }
            else
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Forbidden(nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess), "This account type cannot be created via this portal."));
            }
        }

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterOrCandidatePasswordAsync(
            Register_ResetPassword_VoterOrCandidate_DTO dto,
            CancellationToken ct = default)
        {
            // -----------------------------------------------------------------------
            // PHASE 1: PRE-FLIGHT CHECKS (Fast & Cheap)
            // -----------------------------------------------------------------------

            if (dto.NewPassword != dto.ConfirmPassword)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Password_Mismatch), "Passwords do not match."));

            // Find existing user by username
            var user = await _userManager.FindByNameAsync(dto.UserName!);
            if (user == null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Forbidden(nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess), "User account not available for resetting password.Check the data entered or Contact Support."));

            // -----------------------------------------------------------------------
            // PHASE 2: ROLE BRANCHING
            // -----------------------------------------------------------------------

            // Get roles associated with this user
            var userRoles = await _userManager.GetRolesAsync(user);

            // OPTION A: Logic for VOTER
            if (userRoles.Contains(RoleTypesEnum.Voter.ToString()))
            {
                // -----------------------------------------------------------------------
                // PHASE 2: GET THE TRUTH (The Gateway Call)
                // -----------------------------------------------------------------------
                // Verify the voter exists and the token is valid in Government System

                var verifyRequest = new NeoVoting_GetVoterRequestDTO
                {
                    NationalId = dto.NationalId,
                    VotingToken = dto.VotingOrNominationToken
                };

                var verifyResult = await _govGateway.GetVoterAsync(verifyRequest, ct);

                if (verifyResult.IsFailure)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(verifyResult.Error);
                }

                var govData = verifyResult.Value;

                // -----------------------------------------------------------------------
                // PHASE 3: APPLY BUSINESS RULES
                // -----------------------------------------------------------------------
                // Government System already checked these during GetVoter
                /*if (!govData.ValidToken)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Voter.InvalidToken", "Voting token is invalid. Cannot reset password."));

                if (!govData.IsRegistered)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Voter.NotRegistered", "Voter is not registered in the system."));*/

                // Verify the username matches what's recorded in Gov System
                if (!string.Equals(govData.RegisteredUsername, dto.UserName, StringComparison.OrdinalIgnoreCase))
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Forbidden(nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess), "User account not available for resetting password.Check the data entered or Contact Support."));

                // -----------------------------------------------------------------------
                // PHASE 4: LOCAL COMMIT (Reset Password)
                // -----------------------------------------------------------------------

                // 1. Generate a reset token (Allows us to force-reset without knowing the old password)
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // 2. Perform the reset (This validates the new password policy AND saves to DB)
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword!);

                if (!resetResult.Succeeded)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.PasswordReset_Failed), resetResult.Errors.First().Description));
                }

                // Invalidate refresh token to force re-login
                user.InvalidateRefreshToken();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.PasswordReset_Failed), updateResult.Errors.First().Description));
                }
                // -----------------------------------------------------------------------
                // PHASE 5: SUCCESS
                // -----------------------------------------------------------------------
                // No distributed commit needed for password reset - it's a local-only operation

                return Result<Registration_ResetPassword_ResponseDTO>.Success(
                    MapToResponseDTO(user, RoleTypesEnum.Voter.ToString()));
            }
            // OPTION B: Logic for CANDIDATE
            else if (userRoles.Contains(RoleTypesEnum.Candidate.ToString()))
            {
                // -----------------------------------------------------------------------
                // PHASE 2: GET THE TRUTH (The Gateway Call)
                // -----------------------------------------------------------------------
                // Verify the voter exists and the token is valid in Government System

                var verifyRequest = new NeoVoting_GetCandidateRequestDTO
                {
                    NationalId = dto.NationalId,
                    NominationToken = dto.VotingOrNominationToken
                };

                var verifyResult = await _govGateway.GetCandidateAsync(verifyRequest, ct);

                if (verifyResult.IsFailure)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(verifyResult.Error);
                }

                var govData = verifyResult.Value;

                // -----------------------------------------------------------------------
                // PHASE 3: APPLY BUSINESS RULES
                // -----------------------------------------------------------------------
                // Government System already checked these during GetVoter
                /*if (!govData.ValidToken)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Voter.InvalidToken", "Voting token is invalid. Cannot reset password."));

                if (!govData.IsRegistered)
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure("Voter.NotRegistered", "Voter is not registered in the system."));*/

                // Verify the username matches what's recorded in Gov System
                if (!string.Equals(govData.RegisteredUsername, dto.UserName, StringComparison.OrdinalIgnoreCase))
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Forbidden(nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess), "User account not available for resetting password.Check the data entered or Contact Support."));

                // -----------------------------------------------------------------------
                // PHASE 4: LOCAL COMMIT (Reset Password)
                // -----------------------------------------------------------------------

                // 1. Generate a reset token (Allows us to force-reset without knowing the old password)
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // 2. Perform the reset (This validates the new password policy AND saves to DB)
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword!);

                if (!resetResult.Succeeded)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.PasswordReset_Failed), resetResult.Errors.First().Description));
                }

                // Invalidate refresh token to force re-login
                user.InvalidateRefreshToken();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.PasswordReset_Failed), updateResult.Errors.First().Description));
                }
                // -----------------------------------------------------------------------
                // PHASE 5: SUCCESS
                // -----------------------------------------------------------------------
                // No distributed commit needed for password reset - it's a local-only operation

                return Result<Registration_ResetPassword_ResponseDTO>.Success(
                    MapToResponseDTO(user, RoleTypesEnum.Candidate.ToString()));
            }

            else
            {
                // OPTION C: Security Fallback (User exists but has neither role, e.g., an Admin)
                // We block them here to prevent unauthorized role resets via this public endpoint.
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Forbidden(nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess), "This account type cannot be reset via this portal."));
            }
           
        }





        #region important-comments-notes
        // CHANGED: We need the userId (or Principal) to know WHO to logout.
        // We cannot rely on SignOutAsync because that only kills Cookies.
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
        #endregion
    }
}