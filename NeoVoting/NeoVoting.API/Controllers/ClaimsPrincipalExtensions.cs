using System;
using System.Security.Claims;

namespace NeoVoting.API.Controllers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var parsedId) ? parsedId : null;
        }
    }
}
