using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace NeoVoting.Domain.ResultErrorDomain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails400ErrorTypes // Validation / Bad Request
    {
        Active_Election_AlreadyExists,
        Cannot_Complete_CompletedElection,
        Cannot_Start_StartedElection,
        Cannot_Complete_UpcomingElection,
        Cannot_Start_CompletedElection,
        Active_Poll_AlreadyExists,
        Cannot_Complete_CompletedPoll,
        Cannot_Start_StartedPoll,
        Cannot_Complete_UpcomingPoll,

        Cannot_Start_CompletedPoll,
        Poll_InvalidAnswers,
        File_Upload_Error
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails401ErrorTypes // Unauthorized 
    {
        User_InvalidCredentials, // any user in system

        Voter_InvalidCredentials,//Government System

        Candidate_InvalidCredentials,//Government System

        Auth_InvalidToken // JWT Token is invalid or expired
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails403ErrorTypes // Forbidden
    {
        Auth_ForbiddenAccess, // JWT Token is valid but user does not have permission to access the resource
        User_Lockedout
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails404ErrorTypes // Not Found
    {
        Poll_NotFound,
        Election_NotFound,
        CandidateProfile_NotFound
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails409ErrorTypes // Conflict
    {
        Voter_AlreadyRegistered,
        Voter_AlreadyVoted,
        Candidate_AlreadyRegistered,
        CandidateProfile_AlreadyExisted,
        User_DuplicateUsername,
        Election_DuplicateName,

        Poll_DuplicateName
        
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProblemDetails500ErrorTypes // Server Failure
    {
        Server_Error, // NeoVoting Server Error
        GovernmentSystemGateway_Error // Government System Gateway Error
    }

    public enum ProblemDetails429ErrorTypes // Too Many Requests
    {
        RateLimit_Exceeded
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