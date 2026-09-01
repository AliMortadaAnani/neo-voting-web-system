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
        //private readonly RoleManager<ApplicationRole> _roleManager;
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
            //RoleManager<ApplicationRole> roleManager,
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
            //_roleManager = roleManager;
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
            // we will send the refresh token in a secure http-only cookie in AuthController
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
                return Result<bool>.Success(false);
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
            // tell browser to delete the refresh token cookie (done in AuthController)
            return Result<bool>.Success(true);
        }

        public async Task<Result<Authentication_ResponseDTO>> RefreshTokenAsync(RefreshToken_RequestDTO? refreshTokenRequestDTO , string? refreshTokenFromCookie, string? accessTokenFromHeader)
        {
            // 1. Determine which refresh token to use based on your precedence rules:
            //    - If cookie is present, use it.
            //    - Otherwise, fall back to the body token (which might be null/empty).
            string? activeRefreshToken = !string.IsNullOrEmpty(refreshTokenFromCookie)
                ? refreshTokenFromCookie
                : refreshTokenRequestDTO?.RefreshToken;

            // 2. If both were missing/null/empty, then fail.
            if (string.IsNullOrEmpty(activeRefreshToken))
            {
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken), "Refresh token is missing."));
            }

            string? activeAccessToken = !string.IsNullOrEmpty(accessTokenFromHeader)
                ? accessTokenFromHeader
                : refreshTokenRequestDTO?.AccessToken;

            if (string.IsNullOrEmpty(activeAccessToken)) {
                return Result<Authentication_ResponseDTO>.Failure(
                    Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken), "Access token is missing."));
            }

            // Now you can use 'activeRefreshToken' for the rest of your business logic
            // 1. Validate the old Access Token
            var principalResult = _tokenServices.GetPrincipalFromExpiredToken(activeAccessToken);

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

            if (user == null || user.RefreshToken != activeRefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
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

            // we will send the refresh token in a secure http-only cookie in AuthController
            return Result<Authentication_ResponseDTO>.Success(authResponse);
        }
        
        public async Task<Result<RegisterVoterOrCandidate_ResponseDTO>> RegisterVoterOrCandidateAsync(
    RegisterVoterOrCandidate_RequestDTO dto,
    RoleTypesEnum role)
        {
            if (await _userManager.FindByNameAsync(dto.UserName!) != null)
                return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                    Error.Conflict(nameof(ProblemDetails409ErrorTypes.User_DuplicateUsername), "This username is already taken. Please choose a different username."));

           

            if (role == RoleTypesEnum.Voter)
            {
                // -----------------------------------------------------------------------
                // PHASE 2: GET THE TRUTH (The Gateway Call)
                // -----------------------------------------------------------------------
                // We rely on the Government System as the "Source of Truth" for user details.

                var verifyRequest = new GetVoterVerificationRequestDTO
                {
                    NationalId = dto.NationalId!,
                    VotingToken = dto.VotingOrNominationToken!
                };

                // CALL GATEWAY: This handles the HTTP Post, Try/Catch, and JSON Parsing.
                var verifyResult = await _govGateway.VerifyVoterAsync(verifyRequest);

                // If the network failed, or the API returned 404/400/500, we stop here.
                if (verifyResult.IsFailure)
                {
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(verifyResult.Error);
                }

                VoterVerificationResponseDTO govData = verifyResult.Value; // This contains Name, DOB, Eligibility, etc.

                // -----------------------------------------------------------------------
                // PHASE 3: APPLY BUSINESS RULES
                // -----------------------------------------------------------------------
                // The API call succeeded, but does the data satisfy our rules?

                if(await _voterRepository.IsVoterExistByVerificationHashAsync(govData.HashedData)) // Check if the voter is already registered in our system
                {
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                        Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyRegistered), "Voter account already exists."));
                }

                // -----------------------------------------------------------------------
                // PHASE 4: LOCAL COMMIT (Create the Account)
                // -----------------------------------------------------------------------


                ApplicationUser newUser = ApplicationUser
                    .CreateAccount(dto.UserName!);

                // SAVE TO DB
                var createResult = await _userManager.CreateAsync(newUser, dto.NewPassword!);

                if (!createResult.Succeeded)
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                        Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), createResult.Errors.First().Description));

                // await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());
                var assignVoterRoleResult = await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());

                if (!assignVoterRoleResult.Succeeded)
                {
                    // Optional: Cleanup user if role assignment fails
                    await _userManager.DeleteAsync(newUser);
                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Failed to assign voter role."));
                }
                // Now create voter account in our local database with the details from the Government System.
                Voter voter = Voter.Create(
                    govData.FirstName,
                    govData.LastName,
                    govData.DateOfBirth,
                    govData.Gender,
                    govData.Governorate,
                    govData.HashedData,
                    newUser.Id);

                _voterRepository.Add(voter);

                await _unitOfWork.SaveChangesAsync();


                var response = new RegisterVoterOrCandidate_ResponseDTO
                {
                    ApplicationUserId = newUser.Id,
                    AccountId = voter.Id,
                    UserName = newUser.UserName,
                    FirstName = voter.FirstName,
                    LastName = voter.LastName,
                    DateOfBirth = voter.DateOfBirth,
                    Gender = voter.Gender,
                    Governorate = voter.Governorate,
                    Role = RoleTypesEnum.Voter
                };

                return Result<RegisterVoterOrCandidate_ResponseDTO>.Success(response);
            }
            else
                if (role == RoleTypesEnum.Candidate)
                {
                    // -----------------------------------------------------------------------
                    // PHASE 2: GET THE TRUTH (The Gateway Call)
                    // -----------------------------------------------------------------------
                    // We rely on the Government System as the "Source of Truth" for user details.

                    var verifyRequest = new GetCandidateVerificationRequestDTO
                    {
                        NationalId = dto.NationalId!,
                        NominationToken = dto.VotingOrNominationToken!
                    };

                    // CALL GATEWAY: This handles the HTTP Post, Try/Catch, and JSON Parsing.
                    var verifyResult = await _govGateway.VerifyCandidateAsync(verifyRequest);

                    // If the network failed, or the API returned 404/400/500, we stop here.
                    if (verifyResult.IsFailure)
                    {
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(verifyResult.Error);
                    }

                    CandidateVerificationResponseDTO govData = verifyResult.Value; // This contains Name, DOB, Eligibility, etc.

                    // -----------------------------------------------------------------------
                    // PHASE 3: APPLY BUSINESS RULES
                    // -----------------------------------------------------------------------
                    // The API call succeeded, but does the data satisfy our rules?

                    if (await _candidateRepository.IsCandidateExistByVerificationHashAsync(govData.HashedData)) // Check if the candidate is already registered in our system
                    {
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                            Error.Conflict(nameof(ProblemDetails409ErrorTypes.Candidate_AlreadyRegistered), "Candidate account already exists."));
                    }

                    // -----------------------------------------------------------------------
                    // PHASE 4: LOCAL COMMIT (Create the Account)
                    // -----------------------------------------------------------------------


                    ApplicationUser newUser = ApplicationUser
                        .CreateAccount(dto.UserName!);

                    // SAVE TO DB
                    var createResult = await _userManager.CreateAsync(newUser, dto.NewPassword!);

                    if (!createResult.Succeeded)
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                            Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), createResult.Errors.First().Description));

                    // await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());
                    var assignVoterRoleResult = await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Candidate.ToString());

                    if (!assignVoterRoleResult.Succeeded)
                    {
                        // Optional: Cleanup user if role assignment fails
                        await _userManager.DeleteAsync(newUser);
                        return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Failed to assign candidate role."));
                    }
                    // Now create candidate account in our local database with the details from the Government System.
                    Candidate candidate = Candidate.Create(
                        govData.FirstName,
                        govData.LastName,
                        govData.DateOfBirth,
                        govData.Gender,
                        govData.Governorate,
                        govData.HashedData,
                        newUser.Id);

                    _candidateRepository.Add(candidate);

                    await _unitOfWork.SaveChangesAsync();

                    var response = new RegisterVoterOrCandidate_ResponseDTO
                    {
                        ApplicationUserId = newUser.Id,
                        AccountId = candidate.Id,
                        UserName = newUser.UserName,
                        FirstName = candidate.FirstName,
                        LastName = candidate.LastName,
                        DateOfBirth = candidate.DateOfBirth,
                        Gender = candidate.Gender,
                        Governorate = candidate.Governorate,
                        Role = RoleTypesEnum.Candidate
                    };

                    return Result<RegisterVoterOrCandidate_ResponseDTO>.Success(response);
                }

            return Result<RegisterVoterOrCandidate_ResponseDTO>.Failure(
                Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Failed to register this account type."));
        }
       
    }
}