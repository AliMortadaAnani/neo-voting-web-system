using System;
using System.Security.Claims;

namespace NeoVoting.API.Controllers
{
    // Extension methods for ClaimsPrincipal to retrieve user information.
    // We also implemented a CurrentUserService to keep controllers clean from claims retrieval.
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var parsedId) ? parsedId : null;
        }
    }
}
