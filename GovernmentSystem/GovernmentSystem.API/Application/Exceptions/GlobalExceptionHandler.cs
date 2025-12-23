using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.Application.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)

        // CancellationToken represents a cooperative way to stop work early when a request is aborted
        // (e.g., client disconnect, timeout). In ASP.NET Core, you typically accept it as a parameter
        // in controller actions or minimal APIs, then pass it down to async operations like EF Core
        // queries, HttpClient calls, or Task.Delay. This lets the framework signal cancellation so
        // long-running work can exit promptly, freeing resources and improving scalability.
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please contact support.",
                Type = nameof(ProblemDetails500ErrorTypes.Server_Error)
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // We handled it
        }
    }
}