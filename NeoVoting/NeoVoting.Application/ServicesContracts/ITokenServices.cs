using NeoVoting.Application.AuthDTOs;
using NeoVoting.Domain.IdentityEntities;
using System.Security.Claims;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ITokenServices
    {
        /*
      * Practical recommendation

    Keep this service without CancellationToken; it’s primarily token generation and a single quick Identity call.
    Make sure higher-level repos/services/controllers accept and pass a CancellationToken, since those layers talk to the database.

      */

        // Generates the Access Token (JWT) and a raw Refresh Token
        Task<AuthenticationResponse> CreateTokensAsync(ApplicationUser? user, CancellationToken cancellationToken = default);

        // Extracts the user principal (claims) from a token, even if it is expired
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token, CancellationToken cancellationToken = default);
    }
}