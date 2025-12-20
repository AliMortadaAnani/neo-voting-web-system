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
        private readonly ITokenServices _tokenServices;
        private readonly IGovernmentSystemGateway _govGateway;
        private readonly ILogger<AuthServices> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ISystemAuditLogRepository _systemAuditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthServices(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenServices tokenServices, IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AuthServices> logger, RoleManager<ApplicationRole> roleManager, ISystemAuditLogRepository systemAuditLogRepository, IUnitOfWork unitOfWork, IGovernmentSystemGateway governmentSystemGateway)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenServices = tokenServices;
            _govGateway = governmentSystemGateway;
            _logger = logger;
            _roleManager = roleManager;
            _systemAuditLogRepository = systemAuditLogRepository;
            _unitOfWork = unitOfWork;
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
        public async Task<Result<bool>> LogoutAsync(LogoutDTO logoutDTO, CancellationToken cancellationToken = default)
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
        //private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        //{
        //    PropertyNameCaseInsensitive = true
        //};
        //private static string Truncate(string? value, int maxLength = 500)
        //{
        //    if (string.IsNullOrEmpty(value)) return string.Empty;
        //    return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        //}

        public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterAsync(
    RegisterVoterDTO dto,
    CancellationToken ct = default)
        {
            // -----------------------------------------------------------------------
            // PHASE 1: PRE-FLIGHT CHECKS (Fast & Cheap)
            // -----------------------------------------------------------------------
            // We check these first because they don't cost money (no external API calls)
            // and don't modify the database.

            if (dto.NewPassword != dto.ConfirmPassword)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Validation("Password.Mismatch", "Passwords do not match."));

            if (await _userManager.FindByNameAsync(dto.UserName!) != null)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Conflict("User.Exists", "This username is already taken."));

            // 3. Ensure "Voter" Role Exists
            if (await _roleManager.FindByNameAsync(RoleTypesEnum.Voter.ToString()) is null)
            {
                ApplicationRole voterRole;
                try
                {
                    voterRole = ApplicationRole.CreateVoterRole();
                }
                catch (Exception ex)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Validation("Role.CreationFailed", $"Failed to create voter role instance: {ex.Message}"));
                }

                var roleResult = await _roleManager.CreateAsync(voterRole);
                if (!roleResult.Succeeded)
                {
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                        Error.Validation("Role.CreateFailed", roleResult.Errors.First().Description));
                }
            }

            // -----------------------------------------------------------------------
            // PHASE 2: GET THE TRUTH (The Gateway Call)
            // -----------------------------------------------------------------------
            // We rely on the Government System as the "Source of Truth" for user details.

            var verifyRequest = new NeoVoting_GetVoterRequestDTO
            {
                NationalId = dto.NationalId,
                VotingToken = dto.VotingToken
            };

            // CALL GATEWAY: This handles the HTTP Post, Try/Catch, and JSON Parsing.
            var verifyResult = await _govGateway.VerifyVoterAsync(verifyRequest, ct);

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

            if (!govData.EligibleForElection)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Voter.Ineligible", "Voter is not eligible for election."));

            if (!govData.ValidToken)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Voter.InvalidToken", "Voting token is invalid."));

            if (govData.IsRegistered)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Conflict("Voter.AlreadyRegistered", "Voter account already exists."));

            if (govData.Voted)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Voter.HasVoted", "Voter has already cast a vote."));

            // -----------------------------------------------------------------------
            // PHASE 4: LOCAL COMMIT (Create the Account)
            // -----------------------------------------------------------------------
            // We instantiate the user using data from 'govData' (Verified), NOT 'dto' (User input).
            // This prevents a user from registering as "Batman" when their ID says "Bruce Wayne".

            ApplicationUser newUser;
            try
            {
                newUser = ApplicationUser.CreateVoterOrCandidateAccount(
               dto.UserName!,
               govData.FirstName,
               govData.LastName,
               govData.DateOfBirth.ToDateTime(TimeOnly.MinValue), // DateOnly -> DateTime
               govData.Gender,
               (int)govData.GovernorateId
           );
            }
            catch (ArgumentException ex)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Validation("User.CreationFailed", ex.Message));
            }

            // SAVE TO DB
            var createResult = await _userManager.CreateAsync(newUser, dto.NewPassword!);

            if (!createResult.Succeeded)
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Validation("User.CreateFailed", createResult.Errors.First().Description));

            // await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());
            var assignVoterRoleResult = await _userManager.AddToRoleAsync(newUser, RoleTypesEnum.Voter.ToString());

            if (!assignVoterRoleResult.Succeeded)
            {
                // Optional: Cleanup user if role assignment fails
                await _userManager.DeleteAsync(newUser);
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.RoleFailed", "Failed to assign voter role."));
            }

            // -----------------------------------------------------------------------
            // PHASE 5: DISTRIBUTED COMMIT (The Sync)
            // -----------------------------------------------------------------------
            // We successfully created the local account. Now we MUST tell the Government.

            var confirmRequest = new NeoVoting_VoterIsRegisteredRequestDTO
            {
                NationalId = dto.NationalId,
                VotingToken = dto.VotingToken
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
                try
                {
                    // UNDO Phase 4
                    await _userManager.DeleteAsync(newUser);
                }
                catch (Exception ex)
                {
                    // ADDED: Critical logging for "Zombie User" scenario
                    _logger.LogCritical(ex, "CRITICAL: Failed to rollback user creation for NationalID {NationalId}. User exists in NeoVoting but not in Gov System.", dto.NationalId);
                    // We still return failure to the client, but the Admin needs to see this log.
                }

                var errorToReturn = confirmResult.IsFailure
                    ? confirmResult.Error
                    : Error.Failure("GovernmentSystem.LogicError", "Government System failed to confirm registration.");

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
                SystemActionTypesEnum.VOTER_REGISTERED,
                $"Voter registered successfully.",
                null
            );

            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding system audit log for voter {VoterId}", newUser.Id);
                // but do NOT fail the registration
            }

            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(newUser, RoleTypesEnum.Voter.ToString()));
        }

        //--- Helper Method ---
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

        public Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterPasswordAsync(ResetVoterPasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterCandidateAsync(RegisterCandidateDTO registrationDTO, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Registration_ResetPassword_ResponseDTO>> ResetCandidatePasswordAsync(ResetCandidatePasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /*public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterVoterAsync(RegisterVoterDTO voterRegistrationDTO, CancellationToken cancellationToken = default)
        {
            // 1. Validate Passwords Match
            if (voterRegistrationDTO.NewPassword != voterRegistrationDTO.ConfirmPassword)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));
            }

            // 2. Check for Duplicate Username
            var existingUser = await _userManager.FindByNameAsync(voterRegistrationDTO.UserName!);
            if (existingUser != null)
            {
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Conflict("User.Exists", "This username is already taken."));
            }

            // 3. Ensure "Voter" Role Exists
            if (await _roleManager.FindByNameAsync(RoleTypesEnum.Voter.ToString()) is null)
            {
                var voterRole = ApplicationRole.CreateVoterRole();
                await _roleManager.CreateAsync(voterRole);
            }

            var verifyVoterClient = _httpClientFactory.CreateClient();

            // You can move this URL to configuration
            var GovernmentSystemBaseUrl = _configuration["GovernmentSystem:BaseUrl"] ?? "https://localhost:5000";
            var GovernmentSystemVerifyVoterUrl = $"{GovernmentSystemBaseUrl.TrimEnd('/')}/api/external/voters/verify";

            HttpResponseMessage GovernmentSystemVerifyVoterResponse;

            try
            {
                // Create body with two GUIDs
                var verifyVoterRequest = new NeoVoting_GetVoterRequestDTO
                {
                    NationalId = voterRegistrationDTO.NationalId, // replace with your actual value
                    VotingToken = voterRegistrationDTO.VotingToken  // replace with your actual value
                };
                // Send request as JSON body
                GovernmentSystemVerifyVoterResponse = await verifyVoterClient.PostAsJsonAsync(GovernmentSystemVerifyVoterUrl, verifyVoterRequest, _jsonOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling GovernmentSystem voters/verify endpoint.");
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
            }

            var verifyVoterResponseStatusCode = (int)GovernmentSystemVerifyVoterResponse.StatusCode;
            string verifyVoterResponseContent = string.Empty;

            try
            {
                verifyVoterResponseContent = await GovernmentSystemVerifyVoterResponse.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read response content from GovernmentSystem voters/verify .");
                // If we can’t read content, still return a generic failure
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
            }

            // Handle 2xx OK
            if (GovernmentSystemVerifyVoterResponse.IsSuccessStatusCode)
            {
                try
                {
                    var verifyVoterResponse = JsonSerializer.Deserialize<NeoVoting_VoterResponseDTO>(verifyVoterResponseContent, _jsonOptions);

                    if (verifyVoterResponse == null)
                    {
                        _logger.LogWarning("GovernmentSystem voters/verify returned 200 but body was null or invalid.");
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Error.GovernmentSystem", "GovernmentSystem returned null"));
                    }

                    DateTime verifyVoterUserDateOfBirthDateTime = verifyVoterResponse.DateOfBirth.ToDateTime(TimeOnly.MinValue);
                    int verifyVoterUserGovernorateIdInt = (int)verifyVoterResponse.GovernorateId;

                    // 1) Not eligible for election
                    if (!verifyVoterResponse.EligibleForElection)
                    {
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Voter.NotEligibleForElection", "Voter is not eligible for election."));
                    }

                    // 2) Token is not valid
                    if (!verifyVoterResponse.ValidToken)
                    {
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Voter.InvalidToken", "Voter token is not valid."));
                    }

                    // 3) Already registered
                    if (verifyVoterResponse.IsRegistered)
                    {
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Voter.AlreadyRegistered", "Voter is already registered in the system."));
                    }

                    // 4) Already voted
                    if (verifyVoterResponse.Voted)
                    {
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Voter.AlreadyVoted", "Voter has already voted and cannot register with new NeoVoting voter account.(Unexpected Behaviour)"));
                    }

                    var registeredVoter = ApplicationUser.CreateVoterOrCandidateAccount(
                            voterRegistrationDTO.UserName!,
                            verifyVoterResponse.FirstName,
                            verifyVoterResponse.LastName,
                            verifyVoterUserDateOfBirthDateTime,
                            verifyVoterResponse.Gender,
                            verifyVoterUserGovernorateIdInt
                            );

                    var createVoterAccountResult = await _userManager.CreateAsync(registeredVoter, voterRegistrationDTO.NewPassword!);
                    if (!createVoterAccountResult.Succeeded)
                    {
                        var errorMsg = string.Join(", ", createVoterAccountResult.Errors.Select(e => e.Description));
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.CreationFailed", errorMsg));
                    }

                    if (createVoterAccountResult.Succeeded)
                    {
                        // 6. Assign "Voter" Role
                        var assignVoterRoleResult = await _userManager.AddToRoleAsync(registeredVoter, RoleTypesEnum.Voter.ToString());

                        if (!assignVoterRoleResult.Succeeded)
                        {
                            // Optional: Cleanup user if role assignment fails
                            await _userManager.DeleteAsync(registeredVoter);
                            return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.RoleFailed", "Failed to assign voter role."));
                        }

                        var registerVoterClient = _httpClientFactory.CreateClient();

                        var GovernmentSystemRegisterVoterUrl = $"{GovernmentSystemBaseUrl.TrimEnd('/')}/api/external/voters/registered-in-neovoting";

                        HttpResponseMessage GovernmentSystemRegisterVoterResponse;

                        try
                        {
                            // Create body with two GUIDs
                            var registerVoterRequest = new NeoVoting_VoterIsRegisteredRequestDTO
                            {
                                NationalId = voterRegistrationDTO.NationalId, // replace with your actual value
                                VotingToken = voterRegistrationDTO.VotingToken  // replace with your actual value
                            };
                            // Send request as JSON body
                            GovernmentSystemRegisterVoterResponse = await registerVoterClient.PostAsJsonAsync(GovernmentSystemRegisterVoterUrl, registerVoterRequest, _jsonOptions, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error while calling GovernmentSystem voters/registered-in-neovoting endpoint.");
                            return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                                Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
                        }

                        var registerVoterResponseStatusCode = (int)GovernmentSystemRegisterVoterResponse.StatusCode;
                        string registerVoterResponseContent = string.Empty;

                        try
                        {
                            registerVoterResponseContent = await GovernmentSystemRegisterVoterResponse.Content.ReadAsStringAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to read response content from GovernmentSystem voters/registered-in-neovoting .");
                            // If we can’t read content, still return a generic failure
                            return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                                Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
                        }

                        if (GovernmentSystemRegisterVoterResponse.IsSuccessStatusCode)
                        {
                            var registerVoterResponse = JsonSerializer.Deserialize<NeoVoting_VoterResponseDTO>(registerVoterResponseContent, _jsonOptions);

                            if (registerVoterResponse == null)
                            {
                                _logger.LogWarning("GovernmentSystem voters/registered-in-neovoting returned 200 but body was null or invalid.");
                                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                                    Error.Failure("Error.GovernmentSystem", "GovernmentSystem returned null"));
                            }

                            bool isNowRegistered = registerVoterResponse.IsRegistered;
                            if (!isNowRegistered)
                            {
                                _logger.LogWarning("GovernmentSystem voters/registered-in-neovoting returned IsRegistered=false after registration attempt.");
                                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                                    Error.Failure("Error.GovernmentSystem", "GovernmentSystem did not register the voter."));
                            }
                        }
                        else
                        {
                            _logger.LogWarning("GovernmentSystem voters/registered-in-neovoting returned non-success status: {StatusCode}, Content: {Content}",
                                registerVoterResponseStatusCode, Truncate(registerVoterResponseContent));
                            // Depending on requirements, you might want to rollback user creation here
                        }

                        var registerVoterLog = SystemAuditLog.Create(
                            registeredVoter.Id,
                            SystemActionTypesEnum.VOTER_REGISTERED,
                            $"Voter '{registeredVoter.UserName}' registered successfully.",
                            null
                        );

                        var addedLog = await _systemAuditLogRepository.AddSystemAuditLogAsync(registerVoterLog, cancellationToken);
                        if (addedLog == null)
                        {
                            return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Failure("SystemLog.AdditionFailed", "System Log for voter registration could not be added."));
                        }

                        int rowsAdded = await _unitOfWork.SaveChangesAsync();

                        if (rowsAdded == 0)
                        {
                            return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Failure("SystemLog.AdditionFailed", "System Log for voter registration could not be added."));
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
                            Role = RoleTypesEnum.Voter.ToString()
                        };

                        return Result<Registration_ResetPassword_ResponseDTO>.Success(registerResponseDto);
                    }
                }
                catch (JsonException jex)
                {
                    _logger.LogError(jex,
                        "Failed to deserialize GovernmentSystem voters/verify  successful response to NeoVoting_VoterResponseDTO. Content: {Content}",
                        Truncate(verifyVoterResponseContent));
                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Error.GovernmentSystem", "GovernmentSystem is down"));
                }
            }

            // Non-success: 400, 401, 404, 500, etc.
            try
            {
                var problem = JsonSerializer.Deserialize<ProblemDetails>(verifyVoterResponseContent, _jsonOptions);

                // We don't *require* problem != null to handle by status,
                // but if it's there we can use its Title/Detail.
                var title = problem?.Title ?? "GovernmentSystem error";
                var detail = problem?.Detail ?? "An error occurred while verifying voter.";

                switch (verifyVoterResponseStatusCode)
                {
                    case StatusCodes.Status404NotFound:
                        // Voter not found in government system
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Validation("Government.VoterNotFound",
                                detail)); // or a fixed message if you prefer

                    case StatusCodes.Status400BadRequest:
                        // Request was invalid according to fluent validation in gov system
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Validation("Government.InvalidRequest",
                                detail));

                    case StatusCodes.Status401Unauthorized:
                        // Likely API key / auth problem
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Government.Unauthorized",
                                "Unauthorized to call GovernmentSystem (check API key or credentials)."));

                    case StatusCodes.Status500InternalServerError:
                        // Internal error in government VerifyVoter
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Government.InternalError",
                                "GovernmentSystem encountered an internal error while verifying voter."));

                    default:
                        // Any other unexpected status
                        return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                            Error.Failure("Government.UnexpectedStatus",
                                $"GovernmentSystem returned unexpected status code {verifyVoterResponseStatusCode}."));
                }
            }
            catch (JsonException jex)
            {
                _logger.LogWarning(jex,
                    "Failed to deserialize GovernmentSystem error response as ProblemDetails. Status: {StatusCode}, Content: {Content}",
                    verifyVoterResponseStatusCode, Truncate(verifyVoterResponseContent));

                // Fallback when we can't parse the body at all
                return Result<Registration_ResetPassword_ResponseDTO>.Failure(
                    Error.Failure("Government.UnparseableError",
                        $"GovernmentSystem returned status {verifyVoterResponseStatusCode} with an unreadable error body."));
            }
        }*/

        //        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterPasswordAsync(ResetVoterPasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        //        {
        //            // 1. Validate Passwords Match
        //            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
        //            {
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));
        //            }

        //            // 2. Find User
        //            var user = await _userManager.FindByNameAsync(resetPasswordDTO.UserName!);

        //            if (user == null)
        //            {
        //                // Security Best Practice: Usually returns vague error, but for this context "Not Found" is acceptable
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("User.NotFound", "User account not found."));
        //            }

        //            // 3. Validate National ID match (Optional Security Check)
        //            // If the DTO sends a NationalId, verify it matches the User's ID
        //            if (resetPasswordDTO.NationalId.HasValue && user.Id != resetPasswordDTO.NationalId.Value)
        //            {
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.Mismatch", "National ID does not match the username provided."));
        //            }

        //            /*
        //             * NOTE ON VOTING TOKEN:
        //             * The DTO contains 'VotingToken', but there is no 'Voters' table to verify it against,
        //             * and 'ApplicationUser' does not store it.
        //             * Therefore, we cannot perform a token validation here in this isolated Identity context.
        //             * We proceed assuming the caller has been verified by other means or if we ignore the token.
        //             */

        //            // 4. Force Reset Password (Remove Old -> Add New)
        //            // This is an administrative reset/forgot password flow
        //            if (await _userManager.HasPasswordAsync(user))
        //            {
        //                var removeResult = await _userManager.RemovePasswordAsync(user);
        //                if (!removeResult.Succeeded)
        //                {
        //                    return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.RemoveFailed", "Failed to process password reset."));
        //                }
        //            }

        //            var addResult = await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword!);

        //            if (!addResult.Succeeded)
        //            {
        //                var errorMsg = string.Join(", ", addResult.Errors.Select(e => e.Description));
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.ChangeFailed", errorMsg));
        //            }

        //            // 5. Invalidate existing sessions (Security Stamp)
        //            // This ensures any old cookies or tokens (if checking stamp) are invalidated
        //            await _userManager.UpdateSecurityStampAsync(user);

        //            // Also invalidate refresh token to force re-login
        //            user.InvalidateRefreshToken();
        //            await _userManager.UpdateAsync(user);

        //            // 6. Return Response
        //            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(user, RoleTypesEnum.Voter.ToString()));
        //        }

        //public async Task<Result<Registration_ResetPassword_ResponseDTO>> RegisterCandidateAsync(RegisterCandidateDTO registrationDTO, CancellationToken cancellationToken = default)
        //        {
        //            // 1. Retrieve Candidate
        //            var candidate = await _dbContext.Candidates
        //                .FirstOrDefaultAsync(c => c.NationalId == registrationDTO.NationalId, cancellationToken);

        //            // 2. Validations
        //            if (candidate == null)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("Candidate.NotFound", "National ID not found in candidate registry."));

        //            if (candidate.IsRegistered)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Conflict("Candidate.AlreadyRegistered", "This candidate is already registered."));

        //            if (candidate.NominationToken != registrationDTO.NominationToken)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Forbidden("Candidate.InvalidToken", "Invalid Nomination Token."));

        //            // 3. Create Identity User
        //            string userName = registrationDTO.NationalId.ToString()!;
        //            ApplicationUser appUser;
        //            try
        //            {
        //                appUser = ApplicationUser.CreateVoterOrCandidateAccount(
        //                    userName,
        //                    candidate.FirstName,
        //                    candidate.LastName,
        //                    candidate.DateOfBirth,
        //                    candidate.Gender,
        //                    candidate.GovernorateId
        //                );
        //            }
        //            catch (ArgumentException ex)
        //            {
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Candidate.DataInvalid", ex.Message));
        //            }

        //            // 4. Save Identity User
        //            var createResult = await _userManager.CreateAsync(appUser);
        //            if (!createResult.Succeeded)
        //            {
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.CreationFailed", createResult.Errors.First().Description));
        //            }

        //            // 5. Assign Role
        //            var roleResult = await _userManager.AddToRoleAsync(appUser, RoleTypesEnum.Candidate.ToString());
        //            if (!roleResult.Succeeded)
        //            {
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("User.RoleFailed", "Failed to assign candidate role."));
        //            }

        //            // 6. Mark Candidate as Registered
        //            candidate.IsRegistered = true;
        //            await _dbContext.SaveChangesAsync(cancellationToken);

        //            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(appUser, RoleTypesEnum.Candidate.ToString()));
        //        }

        //        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetVoterPasswordAsync(ResetVoterPasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        //        {
        //            // 1. Validate Password Match
        //            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));

        //            // 2. Validate against Voter Registry (Security Check: Must have valid Token)
        //            var voter = await _dbContext.Voters
        //                .FirstOrDefaultAsync(v => v.NationalId == resetPasswordDTO.NationalId, cancellationToken);

        //            if (voter == null)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("Voter.NotFound", "Voter not found."));

        //            if (voter.VotingToken != resetPasswordDTO.VotingToken)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Forbidden("Voter.InvalidToken", "Invalid Voting Token. Cannot reset password."));

        //            // 3. Find Identity User
        //            var user = await _userManager.FindByNameAsync(resetPasswordDTO.UserName!);
        //            if (user == null)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("User.NotFound", "User account not found."));

        //            // 4. Force Reset Password (Remove old, Add new)
        //            // Since we validated the VotingToken, we trust this request.
        //            if (await _userManager.HasPasswordAsync(user))
        //            {
        //                await _userManager.RemovePasswordAsync(user);
        //            }

        //            var addPassResult = await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword!);

        //            if (!addPassResult.Succeeded)
        //            {
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.ChangeFailed", addPassResult.Errors.First().Description));
        //            }

        //            // 5. Important: Update Security Stamp to invalidate old cookies/tokens
        //            await _userManager.UpdateSecurityStampAsync(user);

        //            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(user, RoleTypesEnum.Voter.ToString()));
        //        }

        //        public async Task<Result<Registration_ResetPassword_ResponseDTO>> ResetCandidatePasswordAsync(ResetCandidatePasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        //        {
        //            // 1. Validate Password Match
        //            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.Mismatch", "Passwords do not match."));

        //            // 2. Validate against Candidate Registry
        //            var candidate = await _dbContext.Candidates
        //                .FirstOrDefaultAsync(c => c.NationalId == resetPasswordDTO.NationalId, cancellationToken);

        //            if (candidate == null)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("Candidate.NotFound", "Candidate not found."));

        //            if (candidate.NominationToken != resetPasswordDTO.NominationToken)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Forbidden("Candidate.InvalidToken", "Invalid Nomination Token. Cannot reset password."));

        //            // 3. Find Identity User
        //            var user = await _userManager.FindByNameAsync(resetPasswordDTO.UserName!);
        //            if (user == null)
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.NotFound("User.NotFound", "User account not found."));

        //            // 4. Force Reset Password
        //            if (await _userManager.HasPasswordAsync(user))
        //            {
        //                await _userManager.RemovePasswordAsync(user);
        //            }

        //            var addPassResult = await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword!);

        //            if (!addPassResult.Succeeded)
        //            {
        //                return Result<Registration_ResetPassword_ResponseDTO>.Failure(Error.Validation("Password.ChangeFailed", addPassResult.Errors.First().Description));
        //            }

        //            await _userManager.UpdateSecurityStampAsync(user);

        //            return Result<Registration_ResetPassword_ResponseDTO>.Success(MapToResponseDTO(user, RoleTypesEnum.Candidate.ToString()));
        //        }
    }
}