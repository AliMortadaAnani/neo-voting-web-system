using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace GovernmentSystem.API.Domain.ResultErrorDomain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails400ErrorTypes // Validation / Bad Request
    {
        Paging_InvalidInput
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails401ErrorTypes // Unauthorized (Auth & Credentials)
    {
        Admin_InvalidCredentials,
        Voter_InvalidCredentials,
        Candidate_InvalidCredentials,

        // System - Middleware
        Auth_UnauthorizedAccess,  // For Cookie Middleware (Missing Cookie)

        Auth_InvalidApiKey,       // For Filter (Wrong API Key)
        Auth_TokenMissing         // For Filter
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails403ErrorTypes // Forbidden
    {
        Auth_ForbiddenAccess,      // For Cookie Middleware (Role mismatch)
        IpWhitelist_ForbiddenIP,    // For IP Whitelist Middleware
        IpWhitelist_RestrictedEndpoint // For IP Whitelist Middleware (External trying to access Admin)
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails404ErrorTypes // Not Found
    {
        Voter_NotFound,
        Candidate_NotFound,
        Citizen_NotFound,
        Paging_OutOfBounds
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails409ErrorTypes // Conflict
    {
        Citizen_AlreadyRegistered,   // Citizen auto generated unique NationalId // cannot conflict
        Candidate_AlreadyRegistered,// For Voter / Candidate
        Voter_AlreadyRegistered
    }

    public enum ProblemDetails429ErrorTypes // Too Many Requests
    {
        RateLimit_Exceeded
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails500ErrorTypes // Server Failure
    {
        Server_Error,             // Generic Global Handler
        Server_ConfigurationError,// Missing API Key in Config
        Voter_OperationFailed,    // Add/Delete in repos returned unexpected output (omitted)
        Candidate_OperationFailed,
        Citizen_OperationFailed
    }

    // ==============================================================================
    // 400 Bad Request (Logic / Validation)
    // ==============================================================================
    public class BadRequest400ProblemDetails : ProblemDetails
    {
        /// <summary>
        /// The specific validation error code.
        /// </summary>
        [JsonPropertyName("type")] // Maps to the standard "type" JSON field
        public new ProblemDetails400ErrorTypes Type { get; set; }

        [JsonIgnore]
        public new IDictionary<string, object>? Extensions { get; }
    }

    // ==============================================================================
    // 401 Unauthorized (Auth Failed)
    // ==============================================================================
    public class Unauthorized401ProblemDetails : ProblemDetails
    {
        [JsonPropertyName("type")]
        public new ProblemDetails401ErrorTypes Type { get; set; }

        [JsonIgnore]
        public new IDictionary<string, object>? Extensions { get; }
    }

    // ==============================================================================
    // 403 Forbidden (Permissions)
    // ==============================================================================
    public class Forbidden403ProblemDetails : ProblemDetails
    {
        [JsonPropertyName("type")]
        public new ProblemDetails403ErrorTypes Type { get; set; }

        [JsonIgnore]
        public new IDictionary<string, object>? Extensions { get; }
    }

    // ==============================================================================
    // 404 Not Found
    // ==============================================================================
    public class NotFound404ProblemDetails : ProblemDetails
    {
        [JsonPropertyName("type")]
        public new ProblemDetails404ErrorTypes Type { get; set; }

        [JsonIgnore]
        public new IDictionary<string, object>? Extensions { get; }
    }

    // ==============================================================================
    // 409 Conflict (Duplicates / State)
    // ==============================================================================
    public class Conflict409ProblemDetails : ProblemDetails
    {
        [JsonPropertyName("type")]
        public new ProblemDetails409ErrorTypes Type { get; set; }

        [JsonIgnore]
        public new IDictionary<string, object>? Extensions { get; }
    }

    public class TooManyRequests429ProblemDetails : ProblemDetails
    {
        [JsonPropertyName("type")]
        public new ProblemDetails429ErrorTypes Type { get; set; }

        [JsonIgnore]
        public new IDictionary<string, object>? Extensions { get; }
    }

    // ==============================================================================
    // 500 Server Error (Crashes)
    // ==============================================================================
    public class ServerError500ProblemDetails : ProblemDetails
    {
        [JsonPropertyName("type")]
        public new ProblemDetails500ErrorTypes Type { get; set; }

        [JsonIgnore]
        public new IDictionary<string, object>? Extensions { get; }
    }
}