using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Middlewares
{
    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IpWhitelistMiddleware> _logger;

        public IpWhitelistMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<IpWhitelistMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string path = context.Request.Path;

            // Only filter API endpoints. Allow Swagger UI or static files to pass through.
            if (!path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var remoteIp = context.Connection.RemoteIpAddress;
            string incomingIp = remoteIp?.ToString() ?? "";

            // 1. Load IP Lists
            var adminIps = _configuration.GetSection("SecuritySettings:AdminWhitelist").Get<string[]>() ?? Array.Empty<string>();
            var externalIps = _configuration.GetSection("SecuritySettings:ExternalSystemWhitelist").Get<string[]>() ?? Array.Empty<string>();

            // 2. Check if Admin (Super User)
            // Admins can access EVERYTHING (/api/admin and /api/external)
            if (adminIps.Contains(incomingIp))
            {
                await _next(context);
                return;
            }

            // 3. Check if External System (Restricted User)
            if (externalIps.Contains(incomingIp))
            {
                // They are only allowed to touch /api/external
                if (path.StartsWith("/api/external", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }
                else
                {
                    // External IP tried to access /api/admin or other protected routes
                    _logger.LogWarning($"Security Alert: External System IP {incomingIp} tried to access restricted path {path}");
                    await WriteProblemDetailsAsync(
                        context,
                        statusCode: StatusCodes.Status403Forbidden,
                        title: "Access denied",
                        detail: "Your IP is authorized only for External APIs.",
                        type: nameof(ProblemDetails403ErrorTypes.IpWhitelist_RestrictedEndpoint));

                    return;
                }
            }

            // 4. Unknown IP
            _logger.LogWarning($"Security Alert: Unauthorized access attempt from IP {incomingIp} to {path}");
            await WriteProblemDetailsAsync(
                context,
                statusCode: StatusCodes.Status403Forbidden,
                title: "Access denied",
                detail: "IP not whitelisted.",
                type: nameof(ProblemDetails403ErrorTypes.IpWhitelist_ForbiddenIP));
        }

        private static async Task WriteProblemDetailsAsync(
           HttpContext context,
           int statusCode,
           string title,
           string detail,
           string? type = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Type = type
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}