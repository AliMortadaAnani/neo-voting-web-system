using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenServices(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        /*
     * Practical recommendation

   Keep this service without CancellationToken; it’s primarily token generation and a single quick Identity call.
   Make sure higher-level repos/services/controllers accept and pass a CancellationToken, since those layers talk to the database.
   We can keep the cancellation token in signature in case we use it in future.   
     */


        public async Task<AuthenticationResponse> CreateTokensAsync(ApplicationUser? user, CancellationToken cancellationToken = default)
        {
            // 1. Define Basic Claims
            var claims = new List<Claim>
    {
        // Standard JWT Claims
        new Claim(JwtRegisteredClaimNames.Sub, user!.Id.ToString()), // Subject (User ID)
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique Token ID
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Issued At
        // App Specific Standard Claims
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
        new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString())
    };

            // 2. Add Personal Details (Only if they are not null)

            // First Name -> "given_name"
            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName));
            }

            // Last Name -> "family_name"
            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName));
            }

            // Date of Birth -> "birthdate" (Standard format: YYYY-MM-DD)
            if (user.DateOfBirth.HasValue)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Birthdate, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
            }

            // Gender -> "gender"
            if (user.Gender.HasValue)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Gender, user.Gender.Value.ToString()));
            }

            // Governorate -> Custom Claim "governorateId" (No standard JWT claim for this)
            if (user.GovernorateID.HasValue)
            {
                claims.Add(new Claim("governorateId", user.GovernorateID.Value.ToString()));
            }



            // 3. Add Roles
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.Single(); // throws if not exactly one role
           // foreach (var role in roles)
           // {
                // .NET requires "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" to recognize roles automatically
                // using 'ClaimTypes.Role' ensures this mapping.
                claims.Add(new Claim(ClaimTypes.Role, userRole));
           // }

            // 4. Create Credentials & Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"]!));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                SigningCredentials = creds,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // 5. Generate Refresh Token (Random String)
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenDurationInDays"]!));


            
            


            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = expiry,
                RefreshTokenExpiration = refreshTokenExpiry,
                Role = userRole,
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GovernorateId = user.GovernorateID,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender
            };

        }
        // This method extracts claims from an expired JWT token without validating its lifetime.
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token, CancellationToken cancellationToken = default)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)),

                // IMPORTANT: We want to read the claims even if the token is expired (to validate refresh)
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                // Check if the token was actually signed with HmacSha256
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch
            {
                // Return null if token is malformed or invalid
                return null;
            }
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        
    }
}
