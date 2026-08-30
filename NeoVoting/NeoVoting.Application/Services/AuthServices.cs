using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.RequestDTOs.AuthDTOs;
using NeoVoting.Application.ResponseDTOs.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;
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
        private readonly ICurrentUserServices _currentUserServices;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IVoterRepository _voterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthServices> _logger;

        public AuthServices
            (
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITokenServices tokenServices,
            ILogger<AuthServices> logger,
            RoleManager<ApplicationRole> roleManager,
            IUnitOfWork unitOfWork,
            IGovernmentSystemGateway governmentSystemGateway,
            ICurrentUserServices currentUserServices,
            ICandidateRepository candidateRepository,
            IVoterRepository voterRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenServices = tokenServices;
            _govGateway = governmentSystemGateway;
            _logger = logger;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _currentUserServices = currentUserServices;
            _candidateRepository = candidateRepository;
            _voterRepository = voterRepository;
        }

        public async Task<Result<Authentication_ResponseDTO>> LoginAsync(Login_RequestDTO loginDTO)
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
                        (nameof(ProblemDetails403ErrorTypes.User_Lockedout),
                        "Account is locked due to multiple failed attempts. Try again later."));
                }
                return Result<Authentication_ResponseDTO>.Failure
                    (Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.User_InvalidCredentials),
                    "Invalid user credentials."));
            }

            // 2. FETCH THE USER'S ROLES HERE
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault(); // Assumes a user has a primary role or you pick the first one

            if (string.IsNullOrEmpty(userRole))
            {
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "User has no assigned role."));
            }

            // 1. Try to parse the user's role string into your Enum safely
            if (!Enum.TryParse<RoleTypesEnum>(userRole, ignoreCase: true, out var roleEnum))
            {
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error),
                    $"Role '{userRole}' is invalid."));
            }

            Authentication_ResponseDTO authResponse = null!;

            switch (roleEnum)
            {
                case RoleTypesEnum.Admin:
                    {
                        // Block scope for Admin
                        authResponse = await _tokenServices.CreateAdminTokensAsync(user);
                        break;
                    }

                case RoleTypesEnum.Candidate:
                    {
                        var candidateRecordFromDb = await _candidateRepository.GetByUserIdAsync(user.Id);

                        if(candidateRecordFromDb == null)
                        {
                            return Result<Authentication_ResponseDTO>.Failure(
                                Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Candidate record not found for the user with candidate role."));
                        }

                        authResponse = await _tokenServices.CreateCandidateTokensAsync(user, candidateRecordFromDb);

                        break;
                    }

                case RoleTypesEnum.Voter:
                    {
                        var voterRecordFromDb = await _voterRepository.GetByUserIdAsync(user.Id);

                        if (voterRecordFromDb == null)
                        {
                            return Result<Authentication_ResponseDTO>.Failure(
                                Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Voter record not found for the user with voter role."));
                        }

                        authResponse = await _tokenServices.CreateVoterTokensAsync(user, voterRecordFromDb);

                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleEnum), roleEnum, "Invalid role type.");
            }



            // Helper method in ApplicationUser entity
            user.UpdateRefreshToken(authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), $"Failed to update refresh token: {errors}"));
            }

            return Result<Authentication_ResponseDTO>.Success(authResponse);
        }

        //[Authorize] (only authenticated users should call this unlike other methods Here)
        public async Task<Result<bool>> LogoutAsync()
        {
            // 1. Get authenticated user ID (throws if not authenticated)
            // This is safe because the endpoint requires [Authorize]
            // if it throws here, something is very wrong !!!!
            var userId = _currentUserServices.ApplicationUserId;

            // 2. Find user in database
            var user = await _userManager.FindByIdAsync(userId.ToString()!);

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
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), $"Failed to invalidate refresh token: {errors}"));
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<Authentication_ResponseDTO>> RefreshTokenAsync(RefreshToken_RequestDTO refreshTokenRequestDTO)
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
                    Error.Unauthorized(nameof(ProblemDetails500ErrorTypes.Server_Error), "Token is missing user identity."));
            }

            // 2. Validate the Refresh Token (from DB)
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null || user.RefreshToken != refreshTokenRequestDTO.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
            {
                return Result<Authentication_ResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken), "Invalid token, please login")); // Invalid or Expired Refresh Token
            }

            // 1. FETCH THE USER'S ROLES HERE
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault(); // Assumes a user has a primary role or you pick the first one

            if (string.IsNullOrEmpty(userRole))
            {
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "User has no assigned role."));
            }

            // 1. Try to parse the user's role string into your Enum safely
            if (!Enum.TryParse<RoleTypesEnum>(userRole, ignoreCase: true, out var roleEnum))
            {
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error),
                    $"Role '{userRole}' is invalid."));
            }

            Authentication_ResponseDTO authResponse = null!;

            switch (roleEnum)
            {
                case RoleTypesEnum.Admin:
                    {
                        // Block scope for Admin
                        authResponse = await _tokenServices.CreateAdminTokensAsync(user);
                        break;
                    }

                case RoleTypesEnum.Candidate:
                    {
                        var candidateRecordFromDb = await _candidateRepository.GetByUserIdAsync(user.Id);

                        if (candidateRecordFromDb == null)
                        {
                            return Result<Authentication_ResponseDTO>.Failure(
                                Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Candidate record not found for the user with candidate role."));
                        }

                        authResponse = await _tokenServices.CreateCandidateTokensAsync(user, candidateRecordFromDb);

                        break;
                    }

                case RoleTypesEnum.Voter:
                    {
                        var voterRecordFromDb = await _voterRepository.GetByUserIdAsync(user.Id);

                        if (voterRecordFromDb == null)
                        {
                            return Result<Authentication_ResponseDTO>.Failure(
                                Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Voter record not found for the user with voter role."));
                        }

                        authResponse = await _tokenServices.CreateVoterTokensAsync(user, voterRecordFromDb);

                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleEnum), roleEnum, "Invalid role type.");
            }



            // Helper method in ApplicationUser entity
            user.UpdateRefreshToken(authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), $"Failed to update refresh token: {errors}"));
            }

            return Result<Authentication_ResponseDTO>.Success(authResponse);
        }
        
        public async Task<Result<RegisterVoterOrCandidate_ResponseDTO>> RegisterVoterOrCandidateAsync(
    RegisterVoterOrCandidate_RequestDTO dto,
    RoleTypesEnum role)
        {
            // -----------------------------------------------------------------------
            // PHASE 1: PRE-FLIGHT CHECKS (Fast & Cheap)
            // -----------------------------------------------------------------------
            // We check these first because they don't cost money (no external API calls)
            // and don't modify the database.

            if (dto.NewPassword != dto.ConfirmPassword)
                return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Password_Mismatch), "Passwords do not match."));

            if (await _userManager.FindByNameAsync(dto.UserName!) != null)
                return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                    Error.Conflict(nameof(ProblemDetails409ErrorTypes.User_DuplicateUsername), "This username is already taken."));

            // Ensure "Voter" Role Exists
            if (await _roleManager.FindByNameAsync(RoleTypesEnum.Voter.ToString()) is null)
            {
                ApplicationRole voterRole = ApplicationRole.CreateVoterRole();

                var roleResult = await _roleManager.CreateAsync(voterRole);
                if (!roleResult.Succeeded)
                {
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
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
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.Role_CreationFailed), roleResult.Errors.First().Description));
                }
            }

            if (role == RoleTypesEnum.Voter)
            {
                // -----------------------------------------------------------------------
                // PHASE 2: GET THE TRUTH (The Gateway Call)
                // -----------------------------------------------------------------------
                // We rely on the Government System as the "Source of Truth" for user details.

                var verifyRequest = new NeoVoting_VerifyVoterRequestDTO
                {
                    NationalId = dto.NationalId,
                    VotingToken = dto.VotingOrNominationToken
                };

                // CALL GATEWAY: This handles the HTTP Post, Try/Catch, and JSON Parsing.
                var verifyResult = await _govGateway.VerifyVoterAsync(verifyRequest, ct);

                // If the network failed, or the API returned 404/400/500, we stop here.
                if (verifyResult.IsFailure)
                {
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(verifyResult.Error);
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
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                        Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyRegistered), "Voter account already exists."));

                if (govData.Voted)
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
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
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.UserCreation_Failed), createResult.Errors.First().Description));

                // await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());
                var assignVoterRoleResult = await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());

                if (!assignVoterRoleResult.Succeeded)
                {
                    // Optional: Cleanup user if role assignment fails
                    await _userManager.DeleteAsync(newUser);
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.UserRoleAssignment_Failed),
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

                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(errorToReturn);
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

                return Result<RegisterVoterOrCandidate_ResponseDTO>.Success(MapToResponseDTO(newUser, RoleTypesEnum.Voter.ToString()));
            }
            else
                if (role == RoleTypesEnum.Candidate)
                {
                    // -----------------------------------------------------------------------
                    // PHASE 2: GET THE TRUTH (The Gateway Call)
                    // -----------------------------------------------------------------------

                    var verifyRequest = new NeoVoting_VerifyCandidateRequestDTO
                    {
                        NationalId = dto.NationalId,
                        NominationToken = dto.VotingOrNominationToken
                    };

                    var verifyResult = await _govGateway.VerifyCandidateAsync(verifyRequest, ct);

                    if (verifyResult.IsFailure)
                    {
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(verifyResult.Error);
                    }

                    var govData = verifyResult.Value;

                    // -----------------------------------------------------------------------
                    // PHASE 3: APPLY BUSINESS RULES
                    // -----------------------------------------------------------------------

                    if (govData.IsRegistered)
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
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
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                            Error.Failure(nameof(ProblemDetails500ErrorTypes.UserCreation_Failed), createResult.Errors.First().Description));

                    var assignRoleResult = await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Candidate.ToString());

                    if (!assignRoleResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(newUser);
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
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

                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(errorToReturn);
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

                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Success(
                        MapToResponseDTO(newUser, RoleTypesEnum.Candidate.ToString()));
                }
                else
                {
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                        Error.Forbidden(nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess), "This account type cannot be created via this portal."));
                }
        }

        

       
    }
}