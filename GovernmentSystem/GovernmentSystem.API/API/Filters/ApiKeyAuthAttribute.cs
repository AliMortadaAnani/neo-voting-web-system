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
                // 1. Check if the Header exists
                if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Content = "API Key is missing."
                    };
                    return;
                }

                // 2. Get the Configuration Service to read appsettings
                var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var apiKey = configuration.GetValue<string>(ConfigKey);

                // 3. Server Configuration Check (Safety)
                if (string.IsNullOrEmpty(apiKey))
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Content = "Server security configuration is invalid (API Key not set)."
                    };
                    return;
                }

                // 4. Compare the Keys
                if (!apiKey.Equals(extractedApiKey))
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Content = "Invalid API Key."
                    };
                    return;
                }

                // If we get here, authentication passed. The request proceeds to the Controller.
                await Task.CompletedTask;
            }
        }
    }
