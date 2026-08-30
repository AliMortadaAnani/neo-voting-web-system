using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NeoVoting.Application.ResponseDTOs.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NeoVoting.Application.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;

        public TokenServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ==========================================
        // 1. THE SHARED TOKEN CREATOR (Core Engine)
        // ==========================================
        private string CreateJWT_AccessToken(List<Claim> claims)
        {
            // Convert secret key config to bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Calculate token lifetime expiry
            var expiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"]!));

            // Bundle descriptor settings
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                SigningCredentials = creds,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            // Generate token string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // ==========================================
        // 2. SPECIFIC CLAIM BUILDERS
        // ==========================================

        private List<Claim> CreateAdminClaims(ApplicationUser user)
        {
            return new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("applicationUserId", user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(ClaimTypes.Role, RoleTypesEnum.Admin.ToString()) // Role claim ("Admin")
    };
        }

        private List<Claim> CreateCandidateClaims(ApplicationUser user,Candidate candidate)
        {
            return new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("applicationUserId", user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(ClaimTypes.Role, RoleTypesEnum.Candidate.ToString()), // Role claim ("Candidate")
        
        // Candidate-specific custom claims mapped to your CurrentUserService expectations
        new Claim("accountId", candidate.Id.ToString()),
        new Claim("governorate", ((int)candidate.Governorate).ToString())
    };
        }

        private List<Claim> CreateVoterClaims(ApplicationUser user,Voter voter)
        {
            return new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("applicationUserId", user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(ClaimTypes.Role, RoleTypesEnum.Voter.ToString()), // Role claim ("Voter")
        
        // Voter-specific custom claims mapped to your CurrentUserService expectations
        new Claim("accountId", voter.Id.ToString()),
        new Claim("governorate", ((int)voter.Governorate).ToString())
    };
        }


        // ==========================================
        // 3. WRAPPER / ORCHESTRATOR METHODS
        // ==========================================

        public async Task<Authentication_ResponseDTO> CreateAdminTokensAsync(ApplicationUser user)
        {
            var claims = CreateAdminClaims(user);
            
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"]!));
           
            var accessToken = CreateJWT_AccessToken(claims);

            // 8. Generate a cryptographically secure random sequence for the refresh token
            var refreshToken = GenerateRefreshToken();

            // 9. Calculate the absolute expiration timestamp for the refresh token (usually much longer than access token)
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"]!));


            return new Authentication_ResponseDTO
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiry,

                ApplicationUserId = user.Id,
                UserName = user.UserName,
                Role = RoleTypesEnum.Admin
            };
        }

        public async Task<Authentication_ResponseDTO> CreateCandidateTokensAsync(ApplicationUser user, Candidate candidate)
        {
            var claims = CreateCandidateClaims(user, candidate);

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"]!));

            var accessToken = CreateJWT_AccessToken(claims);

            // 8. Generate a cryptographically secure random sequence for the refresh token
            var refreshToken = GenerateRefreshToken();

            // 9. Calculate the absolute expiration timestamp for the refresh token (usually much longer than access token)
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"]!));


            return new Authentication_ResponseDTO
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiry,

                ApplicationUserId = user.Id,
                UserName = user.UserName,
                Role = RoleTypesEnum.Candidate,

                AccountId = candidate.Id,
                Governorate = candidate.Governorate,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Gender = candidate.Gender,
                DateOfBirth = candidate.DateOfBirth
            };
        }

        public async Task<Authentication_ResponseDTO> CreateVoterTokensAsync(ApplicationUser user, Voter voter)
        {
            var claims = CreateVoterClaims(user, voter);

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"]!));

            var accessToken = CreateJWT_AccessToken(claims);

            // 8. Generate a cryptographically secure random sequence for the refresh token
            var refreshToken = GenerateRefreshToken();

            // 9. Calculate the absolute expiration timestamp for the refresh token (usually much longer than access token)
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"]!));


            return new Authentication_ResponseDTO
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiry,

                ApplicationUserId = user.Id,
                UserName = user.UserName,
                Role = RoleTypesEnum.Voter,

                AccountId = voter.Id,
                Governorate = voter.Governorate,
                FirstName = voter.FirstName,
                LastName = voter.LastName,
                Gender = voter.Gender,
                DateOfBirth = voter.DateOfBirth
            };
        }



        // 11. Method to read and verify claims from an access token even if its lifetime has lapsed
        public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string? token)
        {
            // 12. Setup strict validation rules, explicitly turning off lifetime checks
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)),
                ValidateLifetime = false // Bypasses expiration check so we can extract data from expired tokens safely
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // 13. Attempt to validate the signature and extract user claims principal layout
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                // 14. Ensure the token was signed with the exact expected algorithm to block spoofing/tampering
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Result<ClaimsPrincipal>.Failure(
                        Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken),
                        "Invalid token security algorithm."));
                }

                // 15. Return the successfully extracted claims principal wrapper
                return Result<ClaimsPrincipal>.Success(principal);
            }
            catch
            {
                // 16. Intercept unexpected parsing/signature crashes and convert them to a clean domain error
                return Result<ClaimsPrincipal>.Failure(
                    Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken),
                    "Token is invalid or malformed."));
            }
        }

        // 17. Helper utility to generate a secure random byte stream for refresh tokens
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

