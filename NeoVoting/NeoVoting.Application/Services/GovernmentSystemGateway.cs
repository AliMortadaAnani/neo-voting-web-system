using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class GovernmentSystemGateway : IGovernmentSystemGateway
    {
        // Q: Where are you creating the client?
        // A: We DO NOT create it here with 'new HttpClient()'. 
        //    It is INJECTED by the factory (configured in Program.cs). 
        //    This prevents socket exhaustion issues.
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
            // We delegate the heavy lifting to a generic helper to avoid code duplication
            return await SendVoterRequestAsync("api/external/voters/verify", request, ct);
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> MarkVoterAsRegisteredAsync(NeoVoting_VoterIsRegisteredRequestDTO request, CancellationToken ct)
        {
            return await SendVoterRequestAsync("api/external/voters/registered-in-neovoting", request, ct);
        }

        // =========================================================================================
        // CORE LOGIC: HANDLES REQUESTS, RESPONSES, ERRORS, AND DESERIALIZATION
        // =========================================================================================
        private async Task<Result<NeoVoting_VoterResponseDTO>> SendVoterRequestAsync<TRequest>(string endpoint, TRequest requestDto, CancellationToken ct)
        {
            HttpResponseMessage response;

            // Q: Are deserialization try catch checks needed? 
            // A: YES. We wrap the network call AND reading logic in try-catch.
            //    Why? The server might be down (HttpRequestException) or return HTML instead of JSON (JsonException).
            try
            {
                // Q: Checking if request successful?
                // A: PostAsJsonAsync sends the request. If DNS fails or connection refused, it throws an Exception here.
                response = await _httpClient.PostAsJsonAsync(endpoint, requestDto, _jsonOptions, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network error calling Government System: {Endpoint}", endpoint);
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.Unreachable", "Government System is unreachable."));
            }

            // Q: Where check based on response content (200, 400...)?
            // A: Right here.

            // --- CASE 1: SUCCESS (200-299) ---
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // Q: Where are you receiving response and checking if response successful?
                    // A: We read it here. Note: If the API returns valid 200 OK but the body is HTML (unlikely but possible),
                    //    ReadFromJsonAsync will throw a JsonException. We must catch that.
                    var data = await response.Content.ReadFromJsonAsync<NeoVoting_VoterResponseDTO>(_jsonOptions, ct);

                    if (data == null)
                    {
                        _logger.LogWarning("Gov System returned 200 OK at {Endpoint}, but body was null.", endpoint);
                        return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.NullResponse", "Received empty response from GovernmentSystem."));
                    }

                    return Result<NeoVoting_VoterResponseDTO>.Success(data);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Gov System returned 200 OK at {Endpoint}, but JSON was invalid.", endpoint);
                    return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.BadData", "Government sent invalid data format."));
                }
            }

            // --- CASE 2: API ERROR (400, 401, 404, 500) ---
            // The request reached the server, but the server said "No".

            string content = string.Empty;
            try
            {
                content = await response.Content.ReadAsStringAsync(ct);
            }
            catch { /* Ignore read errors on failure path */ }

            // Try to parse standard "ProblemDetails" error format
            ProblemDetails? problem = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    problem = JsonSerializer.Deserialize<ProblemDetails>(content, _jsonOptions);
                }
            }
            catch
            {
                // Sometimes servers return plain text "Bad Request" instead of JSON. 
                // We swallow this error and use the raw content as the detail.
                _logger.LogWarning("Could not deserialize error response as ProblemDetails: {Content}", content);
            }

            var detail = problem?.Detail ?? content; // Fallback to raw string if ProblemDetails parsing failed
            if (string.IsNullOrEmpty(detail)) detail = response.ReasonPhrase;

            // Q: Where we check based on response status code?
            return (int)response.StatusCode switch
            {
                404 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound("GovernmentSystem.NotFound", detail ?? "Resource not found in Gov System.")),
                400 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Validation("GovernmentSystem.BadRequest", detail ?? "Invalid request sent to Gov System.")),
                401 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("GovernmentSystem.Unauthorized", "NeoVoting is not authorized to call Gov System (Check API Keys).")),
                403 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Forbidden("GovernmentSystem.Forbidden", "Access denied by Gov System(Check IP Adress).")),
                500 => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.InternalError", "Government System crashed internally.")),
                _ => Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("GovernmentSystem.Error", $"Unexpected status code {(int)response.StatusCode}: {detail}"))
            };
        }
    }
}