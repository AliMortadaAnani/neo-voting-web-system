using Microsoft.AspNetCore.Http;
using NeoVoting.Application.ServicesContracts;
using System;
using System.Collections.Generic;
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

        public Guid GetAuthenticatedUserId()
        {
            if (!UserId.HasValue || UserId.Value == Guid.Empty)
            {
                throw new UnauthorizedAccessException(
                    "User must be authenticated to perform this operation.");
            }
            return UserId.Value;
        }

    }
}
