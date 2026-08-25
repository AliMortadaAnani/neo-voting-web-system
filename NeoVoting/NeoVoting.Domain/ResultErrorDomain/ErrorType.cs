namespace NeoVoting.Domain.ResultErrorDomain
{
    public enum ErrorType
    {
        Failure = 0,      // Generic / unexpected domain/infra failure → usually 500
        Validation = 1,   // 400 Bad Request // dealt with FluentValidation
        NotFound = 2,     // 404 Not Found
        Conflict = 3,     // 409 Already exists / conflict
        Forbidden = 4,    // 403 Not allowed even if authenticated (IP whitelisting...)
        Unauthorized = 5, // 401 not authenticated (No valid cookies,jwt,api keys...)
        TooManyRequests = 6, // 429 Too many requests
        None = 7          // Represents "no error" // return when success
    }
}