using Microsoft.AspNetCore.Http;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using System;
using System.Linq;
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

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
        // [Authorize] attribute ensures that the user is authenticated, but this property can be used for additional checks if needed.

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

        public GovernorateIdEnum? Governorate
        {
            get
            {
                var value = User?.FindFirst("governorate")?.Value;

                // Handles if the token stores it as an integer string (e.g., "3") or Enum Name (e.g., "Beirut")
                //if (string.IsNullOrEmpty(value))
                //    return null;

                if (int.TryParse(value, out var intVal) && Enum.IsDefined(typeof(GovernorateIdEnum), intVal))
                {
                    return (GovernorateIdEnum)intVal;
                }

                //if (Enum.TryParse<GovernorateIdEnum>(value, ignoreCase: true, out var enumVal))
                //{
                //    return enumVal;
                //}

                return null;
            }
        }

        public string? GetClaim(string claimType)
        {
            return User?.FindFirst(claimType)?.Value;
        }
    }
}