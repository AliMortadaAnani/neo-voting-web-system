using Microsoft.AspNetCore.Http;
using NeoVoting.Application.ServicesContracts;
using System.Security.Claims;

namespace NeoVoting.Application.Services
{
    public class CurrentUserServices : ICurrentUserServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? ApplicationUserId
        {
            get
            {
                // Check both standard NameIdentifier (sub) and custom claim name if applicable
                var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User?.FindFirst("applicationUserId")?.Value;

                return int.TryParse(value, out var result) ? result : null;
            }
        }

        public int? AccountId
        {
            get
            {
                var value = User?.FindFirst("accountId")?.Value;
                return int.TryParse(value, out var result) ? result : null;
            }
        }

        public string? UserName
        {
            get
            {
                // Checks standard Name claim (or 'unique_name' / 'preferred_username')
                return User?.FindFirst(ClaimTypes.Name)?.Value
                       ?? User?.FindFirst("name")?.Value
                       ?? User?.Identity?.Name;
            }
        }
    }
}