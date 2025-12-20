using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
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

        public async Task<Result<NeoVoting_VoterResponseDTO>> VerifyVoterAsync(NeoVoting_GetVoterRequestDTO request, CancellationToken ct)
        {
            // Verify is usually a POST (sending data to check)
            return await SendVoterRequestAsync(HttpMethod.Post, "api/external/voters/verify", request, ct);
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> MarkVoterAsRegisteredAsync(NeoVoting_VoterIsRegisteredRequestDTO request, CancellationToken ct)
        {
            // Registration is typically a POST (creation) or PUT (update).
            // Currently set to POST based on your previous code, but easily changeable now:
            return await SendVoterRequestAsync(HttpMethod.Put, "api/external/voters/registered-in-neovoting", request, ct);
        }

        // =========================================================================================
        // CORE LOGIC: HANDLES REQUESTS (POST/PUT), RESPONSES, ERRORS, AND DESERIALIZATION
        // =========================================================================================
        private async Task<Result<NeoVoting_VoterResponseDTO>> SendVoterRequestAsync<TRequest>(
            HttpMethod method, // <--- ADDED PARAMETER
            string endpoint,
            TRequest requestDto,
            CancellationToken ct)
        {
            HttpResponseMessage response;

            try
            {
                // Q: Logic to switch between POST and PUT?
                // A: We check the 'method' parameter and call the appropriate extension method.
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
                    _logger.LogError("Unsupported HTTP Method {Method} for SendVoterRequestAsync", method);
                    return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("System.InternalError", $"HTTP Method {method} not supported by Gateway."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network error calling Government System: {Endpoint} [{Method}]", endpoint, method);
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.Unreachable", "Government System is unreachable."));
            }

            // --- CASE 1: SUCCESS (200-299) ---
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var data = await response.Content.ReadFromJsonAsync<NeoVoting_VoterResponseDTO>(_jsonOptions, ct);

                    if (data == null)
                    {
                        _logger.LogWarning("Gov System returned success at {Endpoint}, but body was null.", endpoint);
                        return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.NullResponse", "Received empty response from GovernmentSystem."));
                    }

                    return Result<NeoVoting_VoterResponseDTO>.Success(data);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Gov System returned success at {Endpoint}, but JSON was invalid.", endpoint);
                    return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.BadData", "Government sent invalid data format."));
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
                404 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound(errorCode, errorDesc)),

                400 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Validation(errorCode, errorDesc)),

                401 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(errorCode, "Unauthorized: " + errorDesc)),

                403 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Forbidden(errorCode, errorDesc)),

                500 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure(errorCode, errorDesc)),

                _ => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure(errorCode, $"{response.StatusCode}: {errorDesc}"))
            };
        }
    }
}