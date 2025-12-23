using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace GovernmentSystem.API.Domain.Shared
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails400ErrorTypes // Validation / Bad Request
    {
        Paging_InvalidInput,
        Voter_InvalidUsername,
        Candidate_InvalidUsername
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails401ErrorTypes // Unauthorized (Auth & Credentials)
    {
        // Admin Auth
        Admin_InvalidCredentials, // Was "Admin.NotValid"

        // NeoVoting Logic (Voters)
        Voter_NotEligible,        // Was "Voter.NotValid"

        Voter_InvalidToken,       // Was "Voter.UnauthorizedToken"
        Voter_NotRegistered,      // Was "Voter.NotRegistered" (Trying to vote without account)

        // NeoVoting Logic (Candidates)
        Candidate_NotEligible,

        Candidate_InvalidToken,

        // System / Middleware
        Auth_UnauthorizedAccess,  // For Cookie Middleware (Missing Cookie)

        Auth_InvalidApiKey,       // For Filter (Missing/Wrong API Key)
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
        Voter_NotFound,           // Covers "Voter.Missing" and "Voters.Missing"
        Candidate_NotFound,       // Covers "Candidate.Missing"
        Paging_OutOfBounds
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails409ErrorTypes // Conflict
    {
        Voter_AlreadyRegistered,
        Voter_AlreadyVoted,
        Candidate_AlreadyRegistered
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails500ErrorTypes // Server Failure
    {
        Server_Error,             // Generic Global Handler
        Server_ConfigurationError,// Missing API Key in Config
        Voter_OperationFailed,    // Add/Update/Delete returned 0 rows
        Candidate_OperationFailed
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