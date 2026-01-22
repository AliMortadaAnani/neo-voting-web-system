using Microsoft.AspNetCore.Http;
using NeoVoting.Application.ServicesContracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class CurrentUserServices : ICurrentUserServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User
                    ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userIdClaim, out var id) ? id : null;
            }
        }

        public string? Username
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null) return null;

                // PRIORITY 1: Check standard ASP.NET mapping (ClaimTypes.Name)
                // ASP.NET Core automatically maps "unique_name" -> ClaimTypes.Name
                var name = user.FindFirst(ClaimTypes.Name)?.Value;

                // PRIORITY 2: Check raw JWT claim ("unique_name")
                // In case automatic mapping is disabled in Program.cs
                if (string.IsNullOrEmpty(name))
                {
                    name = user.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
                }

                return name;
            }
        }

        public Guid GetAuthenticatedUserId()
        {
            if (!UserId.HasValue || UserId.Value == Guid.Empty)
            {
                throw new UnauthorizedAccessException(
                    "User must be authenticated to perform this operation.");
            }
            return UserId.Value;
        }

        public string GetAuthenticatedUsername()
        {
            var username = Username;
            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException(
                    "User must be authenticated with a username to perform this operation.");
            }
            return username;
        }
    }
}