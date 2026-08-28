using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ResultErrorDomain;
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

        public async Task<Result<VoterVerificationResponseDTO>> VerifyVoterAsync(GetVoterVerificationRequestDTO request)
        {
            //we append the api key in Registration in API layer
            return await SendRequestToGovernmentSystemAsync<GetVoterVerificationRequestDTO, VoterVerificationResponseDTO>(
                HttpMethod.Post,
                "api/public/voter/verify",
                request);
        }

        // --- CANDIDATE CALLS ---

        public async Task<Result<CandidateVerificationResponseDTO>> VerifyCandidateAsync(GetCandidateVerificationRequestDTO request)
        {
            //we append the api key in Registration in API layer
            return await SendRequestToGovernmentSystemAsync<GetCandidateVerificationRequestDTO, CandidateVerificationResponseDTO>(
                HttpMethod.Post,
                "api/public/candidate/verify",
                request);
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
            TRequest requestDto
            )

        {
            HttpResponseMessage response;

            try
            {
                if (method == HttpMethod.Post)
                {
                    response = await _httpClient.PostAsJsonAsync(endpoint, requestDto, _jsonOptions);
                }
                //else if (method == HttpMethod.Put)
                //{
                //    response = await _httpClient.PutAsJsonAsync(endpoint, requestDto, _jsonOptions);
                //}
                else
                {
                    // Defensive coding: In case a developer passes GET or DELETE or PUT by mistake
                    _logger.LogError("Unsupported HTTP Method {Method} for SendRequestToGovernmentSystemAsync", method);
                    return Result<TResponse>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.GovernmentSystemGateway_Error), $"HTTP Method {method} not supported by Gateway."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network error calling Government System: {Endpoint} [{Method}]", endpoint, method);
                return Result<TResponse>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.GovernmentSystemGateway_Error), "Government System is unreachable."));
            }

            // --- CASE 1: SUCCESS (200-299) ---
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var data = await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);

                    if (data == null)
                    {
                        _logger.LogWarning("Gov System returned success at {Endpoint}, but body was null.", endpoint);
                        return Result<TResponse>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.GovernmentSystemGateway_Error), "Received empty response from GovernmentSystem."));
                    }

                    return Result<TResponse>.Success(data);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Gov System returned success at {Endpoint}, but JSON was invalid.", endpoint);
                    return Result<TResponse>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.GovernmentSystemGateway_Error), "Government sent invalid data format."));
                }
            }

            // --- CASE 2: API ERROR - PROPAGATE REMOTE ERROR ---

            string content = string.Empty;
            try
            {
                content = await response.Content.ReadAsStringAsync();
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