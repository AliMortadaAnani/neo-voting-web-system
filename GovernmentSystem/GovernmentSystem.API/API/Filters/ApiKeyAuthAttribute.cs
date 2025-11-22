using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GovernmentSystem.API.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private const string HeaderName = "X-Gov-Api-Key";
        private const string ConfigKey = "SecuritySettings:ApiKey";

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiKeyAuthAttribute>>();

            logger.LogTrace("ApiKeyAuth.OnAuthorizationAsync started for {Path} at {Time}", context.HttpContext.Request.Path, DateTime.UtcNow);

            // Log all incoming headers for debugging
            foreach (var h in context.HttpContext.Request.Headers)
                logger.LogTrace("Header received: {Key}: {Value}", h.Key, h.Value);

            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
            {
                logger.LogTrace("Header {HeaderName} not found. Rejecting request.", HeaderName);
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "API Key is missing."
                };
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>(ConfigKey);

            logger.LogTrace("Extracted API Key from header: {Extracted}. Configured API Key: {Configured}", extractedApiKey, string.IsNullOrEmpty(apiKey) ? "<null or empty>" : "<hidden>");

            if (string.IsNullOrEmpty(apiKey))
            {
                logger.LogTrace("API key is empty or missing in server config.");
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Content = "Server security configuration is invalid (API Key not set)."
                };
                return;
            }

            if (!apiKey.Equals(extractedApiKey))
            {
                logger.LogTrace("API key mismatch. Provided: {Provided}, Expected: {Expected}.", extractedApiKey, "<hidden>");
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "Invalid API Key."
                };
                return;
            }

            logger.LogTrace("API Key validation succeeded for {Path}", context.HttpContext.Request.Path);

            await Task.CompletedTask;
        }
    }
}