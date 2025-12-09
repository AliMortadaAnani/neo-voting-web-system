using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Controllers
{
    /*
     * [HttpPost("verify")]
public async Task<IActionResult> VerifyVoter(VerifyRequest request)
{
    // Validations (FluentValidation) ran automatically before this line.

    var result = await _voterService.VerifyVoterAsync(request.NationalId, request.Token);

    // One line handling!
    return HandleResult(result);
}
     */

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result,bool Created = false)
        {
            if (result.IsSuccess)
            {   
                if(Created)
                {
                    // This returns HTTP 201 with the JSON body, but no Location header
                    return StatusCode(StatusCodes.Status201Created, result.Value);
                }
                return Ok(result.Value);
            }

            // Convert our "Error" object into standard "ProblemDetails"
            var problemDetails = new ProblemDetails
            {
                Title = GetTitle(result.Error.Type),
                Detail = result.Error.Description,
                Type = result.Error.Code,
                Status = GetStatusCode(result.Error.Type),
                Instance = HttpContext.Request.Path
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        private static int GetStatusCode(ErrorType errorType) => errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        private static string GetTitle(ErrorType errorType) => errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.Unauthorized => "Unauthorized",
            _ => "Server Failure"
        };
    }
}