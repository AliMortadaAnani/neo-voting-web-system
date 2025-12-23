using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GovernmentSystem.API.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private const string HeaderName = "X-Gov-Api-Key";
        private const string ConfigKey = "SecuritySettings:ApiKey";

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "API Key is missing.",
                    Type = nameof(ProblemDetails401ErrorTypes.Auth_TokenMissing)
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };

                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>(ConfigKey);

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server configuration error",
                    Detail = "Server security configuration is invalid (API Key not set).",
                    Type = nameof(ProblemDetails500ErrorTypes.Server_ConfigurationError)
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                return;
            }

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "Invalid API Key.",
                    Type = nameof(ProblemDetails401ErrorTypes.Auth_InvalidApiKey)
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };

                return;
            }

            await Task.CompletedTask;
        }
    }
}