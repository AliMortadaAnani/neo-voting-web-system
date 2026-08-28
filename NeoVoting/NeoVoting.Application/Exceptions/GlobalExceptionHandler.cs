using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Exceptions
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
            CancellationToken cancellationToken = default
           )

        {
            _logger.LogError(exception, "GlobalExceptionHandler: Exception occurred - Message: {Message}, Type: {ExceptionType}", exception.Message, exception.GetType().Name);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please contact support.",
                Type = nameof(ProblemDetails500ErrorTypes.Server_Error)
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError("GlobalExceptionHandler: Returning 500 error response to client at path {Path}", httpContext.Request.Path);
            await httpContext.Response.WriteAsJsonAsync(problemDetails);

            return true; // We handled it
        }
    }
}