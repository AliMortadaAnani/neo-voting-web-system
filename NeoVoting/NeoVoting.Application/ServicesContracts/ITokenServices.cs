using NeoVoting.Application.AuthDTOs;
using NeoVoting.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ITokenServices
    {
        // Generates the Access Token (JWT) and a raw Refresh Token
        Task<AuthenticationResponse> CreateTokensAsync(ApplicationUser user);

        // Extracts the user principal (claims) from a token, even if it is expired
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
