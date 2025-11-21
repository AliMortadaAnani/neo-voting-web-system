using Microsoft.AspNetCore.Http;


namespace GovernmentSystem.API.API.Middlewares
{
    

    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _whitelist;

        public IpWhitelistMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            // Read allowed IPs from configuration, e.g. ["127.0.0.1"]
            _whitelist = configuration.GetSection("IpWhitelist").Get<HashSet<string>>() ?? new HashSet<string> { "" };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "";
            if (!_whitelist.Contains(remoteIp))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden: Your IP is not allowed.");
                return;
            }
            await _next(context);
        }
    }
}
