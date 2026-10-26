using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.ServicesContracts;
using System.Security.Claims;

namespace NeoVoting.Application.Services
{
    public class CurrentUserServices : ICurrentUserServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserServices> _logger;

        public CurrentUserServices(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserServices> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? ApplicationUserId
        {
            get
            {
                _logger.LogInformation("Resolving application user ID from the current request claims.");
                var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User?.FindFirst("applicationUserId")?.Value;

                if (int.TryParse(value, out var parsed))
                {
                    _logger.LogInformation("Application user ID resolution result: {Result}", parsed);
                    return parsed;
                }

                _logger.LogInformation("Application user ID resolution result: <none>");
                return null;
            }
        }

        public int? AccountId
        {
            get
            {
                _logger.LogInformation("Resolving account ID from the current request claims.");
                var value = User?.FindFirst("accountId")?.Value;
                if (int.TryParse(value, out var parsed))
                {
                    _logger.LogInformation("Account ID resolution result: {Result}", parsed);
                    return parsed;
                }

                _logger.LogInformation("Account ID resolution result: <none>");
                return null;
            }
        }

        public string? UserName
        {
            get
            {
                _logger.LogInformation("Resolving username from the current request claims.");
                var userName = User?.FindFirst(ClaimTypes.Name)?.Value
                        ?? User?.FindFirst("name")?.Value
                        ?? User?.Identity?.Name;
                _logger.LogInformation("Username resolution result: {UserName}", userName ?? "<none>");
                return userName;
            }
        }
    }
}