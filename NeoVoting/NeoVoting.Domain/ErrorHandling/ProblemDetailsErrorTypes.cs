using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace NeoVoting.Domain.ErrorHandling
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails400ErrorTypes // Validation / Bad Request
    {
        Paging_InvalidInput,
        Voter_InvalidUsername,//Government System
        Candidate_InvalidUsername//Government System
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails401ErrorTypes // Unauthorized (Auth & Credentials)
    {
        // Admin Auth
        Admin_InvalidCredentials,

        
        Voter_NotEligible,//Government System

        Voter_InvalidToken,//Government System
        Voter_NotRegistered,//Government System

        
        Candidate_NotEligible,//Government System

        Candidate_InvalidToken,//Government System

        
        Auth_UnauthorizedAccess,

        Auth_InvalidApiKey,//Government System
        Auth_TokenMissing//Government System
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails403ErrorTypes // Forbidden
    {
        Auth_ForbiddenAccess,
        IpWhitelist_ForbiddenIP,//Government System
        IpWhitelist_RestrictedEndpoint//Government System
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails404ErrorTypes // Not Found
    {
        Voter_NotFound,//Government System
        Candidate_NotFound,//Government System
        Paging_OutOfBounds
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails409ErrorTypes // Conflict
    {
        Voter_AlreadyRegistered,//Government System
        Voter_AlreadyVoted,//Government System
        Candidate_AlreadyRegistered//Government System
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails500ErrorTypes // Server Failure
    {
        Server_Error,
        Server_ConfigurationError,//Government System
        Voter_OperationFailed,//Government System
        Candidate_OperationFailed//Government System
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