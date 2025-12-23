using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
using System.Net;
using System.Net.Http.Json; // Required for PostAsJsonAsync / PutAsJsonAsync
using System.Text.Json;

namespace NeoVoting.Application.Services
{
    public class GovernmentSystemGateway : IGovernmentSystemGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GovernmentSystemGateway> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public GovernmentSystemGateway(HttpClient httpClient, ILogger<GovernmentSystemGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // --- VOTER CALLS ---

        public async Task<Result<NeoVoting_VoterResponseDTO>> GetVoterAsync(NeoVoting_GetVoterRequestDTO request, CancellationToken ct)
        {
            // Controller: [HttpPost("voters/get")]
            return await SendRequestToGovernmentSystemAsync<NeoVoting_GetVoterRequestDTO, NeoVoting_VoterResponseDTO>(
                HttpMethod.Post,
                "api/external/voters/get",
                request,
                ct);
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> MarkVoterAsRegisteredAsync(NeoVoting_VoterIsRegisteredRequestDTO request, CancellationToken ct)
        {
            // Controller: [HttpPut("voters/register")]
            return await SendRequestToGovernmentSystemAsync<NeoVoting_VoterIsRegisteredRequestDTO, NeoVoting_VoterResponseDTO>(
                HttpMethod.Put,
                "api/external/voters/register",
                request,
                ct);
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> MarkVoterAsVotedAsync(NeoVoting_VoterHasVotedRequestDTO request, CancellationToken ct)
        {
            // Controller: [HttpPut("voters/vote")]
            return await SendRequestToGovernmentSystemAsync<NeoVoting_VoterHasVotedRequestDTO, NeoVoting_VoterResponseDTO>(
                HttpMethod.Put,
                "api/external/voters/vote",
                request,
                ct);
        }

        // --- CANDIDATE CALLS ---

        public async Task<Result<NeoVoting_CandidateResponseDTO>> GetCandidateAsync(NeoVoting_GetCandidateRequestDTO request, CancellationToken ct)
        {
            // Controller: [HttpPost("candidates/get")]
            return await SendRequestToGovernmentSystemAsync<NeoVoting_GetCandidateRequestDTO, NeoVoting_CandidateResponseDTO>(
                HttpMethod.Post,
                "api/external/candidates/get",
                request,
                ct);
        }

        public async Task<Result<NeoVoting_CandidateResponseDTO>> MarkCandidateAsRegisteredAsync(NeoVoting_CandidateIsRegisteredRequestDTO request, CancellationToken ct)
        {
            // Controller: [HttpPut("candidates/register")]
            return await SendRequestToGovernmentSystemAsync<NeoVoting_CandidateIsRegisteredRequestDTO, NeoVoting_CandidateResponseDTO>(
                HttpMethod.Put,
                "api/external/candidates/register",
                request,
                ct);
        }

        // --- SYSTEM CALLS ---

        public async Task<Result<bool>> ResetAllVotersVoteStatusAsync(CancellationToken ct)
        {
            // Controller: [HttpPost("reset-vote-status")]
            // This endpoint takes NO body in the controller, but our generic method expects a request DTO.
            // We can pass a dummy object or null if we handle it.
            // Ideally, overload SendRequest... to handle no-body requests.
            // For now, passing 'object' with null value:

            return await SendRequestToGovernmentSystemAsync<object, bool>(
                HttpMethod.Post,
                "api/external/reset-vote-status",
                new { }, // Empty JSON object as body
                ct);
        }


        // =========================================================================================
        // CORE LOGIC: HANDLES REQUESTS (POST/PUT), RESPONSES, ERRORS, AND DESERIALIZATION
        // =========================================================================================

        //public async Task<TResponse> HandleAsync<TRequest, TResponse>(TRequest request)
        private async Task<Result<TResponse>> 
            SendRequestToGovernmentSystemAsync<TRequest, TResponse>
            
            (
            HttpMethod method, 
            string endpoint,
            TRequest requestDto,
            CancellationToken ct
            )

        {
            HttpResponseMessage response;

            try
            { 
                if (method == HttpMethod.Post)
                {
                    response = await _httpClient.PostAsJsonAsync(endpoint, requestDto, _jsonOptions, ct);
                }
                else if (method == HttpMethod.Put)
                {
                    response = await _httpClient.PutAsJsonAsync(endpoint, requestDto, _jsonOptions, ct);
                }
                else
                {
                    // Defensive coding: In case a developer passes GET or DELETE by mistake
                    _logger.LogError("Unsupported HTTP Method {Method} for SendRequestToGovernmentSystemAsync", method);
                    return Result<TResponse>.Failure(Error.Failure("System.InternalError", $"HTTP Method {method} not supported by Gateway."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network error calling Government System: {Endpoint} [{Method}]", endpoint, method);
                return Result<TResponse>.Failure(Error.Failure("GovernmentSystem.Unreachable", "Government System is unreachable."));
            }

            // --- CASE 1: SUCCESS (200-299) ---
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        // If TResponse is bool, return true. If it's an object, return default.
                        // Usually, if we expect data but get 204, it might be an issue, 
                        // but often 204 means "Success, nothing to see here".
                        //Not in our case but kept for reference

                        return Result<TResponse>.Success(default!);
                    }
                    var data = await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, ct);

                    if (data == null)
                    {
                        _logger.LogWarning("Gov System returned success at {Endpoint}, but body was null.", endpoint);
                        return Result<TResponse>.Failure(Error.Failure("GovernmentSystem.NullResponse", "Received empty response from GovernmentSystem."));
                    }

                    return Result<TResponse>.Success(data);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Gov System returned success at {Endpoint}, but JSON was invalid.", endpoint);
                    return Result<TResponse>.Failure(Error.Failure("GovernmentSystem.BadData", "Government sent invalid data format."));
                }
            }

            // --- CASE 2: API ERROR - PROPAGATE REMOTE ERROR ---

            string content = string.Empty;
            try
            {
                content = await response.Content.ReadAsStringAsync(ct);
            }
            catch { /* Ignore */ }

            ProblemDetails? problem = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    problem = JsonSerializer.Deserialize<ProblemDetails>(content, _jsonOptions);
                }
            }
            catch { /* Ignore JSON parse errors */ }

            // 1. EXTRACT ERROR DESCRIPTION
            // Prefer the 'Detail' from ProblemDetails, fallback to raw content, fallback to Status Phrase
            string errorDesc = problem?.Detail ?? content;
            if (string.IsNullOrWhiteSpace(errorDesc)) errorDesc = response.ReasonPhrase ?? "Unknown External Error";

            // 2. EXTRACT ERROR CODE
            // Prefer 'Title' (often used for codes like "Voter.NotFound"), fallback to 'Type', fallback to generic
            string errorCode = problem?.Type ?? problem?.Title ?? "GovernmentSystem.Error";

            // 3. MAP STATUS CODE BUT KEEP ORIGINAL MESSAGE
            return (int)response.StatusCode switch
            {
                // We use the 'errorCode' and 'errorDesc' from the external system directly
                404 => Result<TResponse>.Failure(Error.NotFound(errorCode, errorDesc)),

                400 => Result<TResponse>.Failure(Error.Validation(errorCode, errorDesc)),

                401 => Result<TResponse>.Failure(Error.Unauthorized(errorCode, "Unauthorized: " + errorDesc)),

                403 => Result<TResponse>.Failure(Error.Forbidden(errorCode, errorDesc)),

                500 => Result<TResponse>.Failure(Error.Failure(errorCode, errorDesc)),

                _ => Result<TResponse>.Failure(Error.Failure(errorCode, $"{response.StatusCode}: {errorDesc}"))
            };
        }
    }
}