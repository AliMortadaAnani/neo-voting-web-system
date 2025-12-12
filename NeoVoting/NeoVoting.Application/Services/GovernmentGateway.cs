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
    public class GovernmentGateway : IGovernmentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GovernmentGateway> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        // Use Typed Client configuration in Program.cs to inject base URL automatically
        public GovernmentGateway(HttpClient httpClient, ILogger<GovernmentGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> VerifyVoterAsync(NeoVoting_GetVoterRequestDTO requestDTO, CancellationToken ct)
        {
            try
            {
                var request = new { requestDTO.NationalId, requestDTO.VotingToken };
                var response = await _httpClient.PostAsJsonAsync("api/external/voters/verify", request, ct);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<NeoVoting_VoterResponseDTO>(_jsonOptions, ct);
                    return Result<NeoVoting_VoterResponseDTO>.Success(data!);
                }

                return await HandleErrorResponse<NeoVoting_VoterResponseDTO>(response, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gov System Verify call failed.");
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("System.GovDown", "Government system unreachable."));
            }
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> MarkVoterAsRegisteredAsync(NeoVoting_VoterIsRegisteredRequestDTO requestDTO, CancellationToken ct)
        {
            try
            {
                var request = new { requestDTO.NationalId,requestDTO.VotingToken };
                var response = await _httpClient.PostAsJsonAsync("api/external/voters/registered-in-neovoting", request, ct);

                if (response.IsSuccessStatusCode)
                {
                    // We just need to know if the Gov system accepted it (IsRegistered = true)
                    var data = await response.Content.ReadFromJsonAsync<NeoVoting_VoterResponseDTO>(_jsonOptions, ct);
                    if (data != null && data.IsRegistered)
                        return Result<NeoVoting_VoterResponseDTO>.Success(NeoVoting_VoterResponseDTO);

                    return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Failure("Gov.LogicError", "Government system did not mark voter as registered."));
                }

                return await HandleErrorResponse<NeoVoting_VoterResponseDTO>(response, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gov System Register call failed.");
                return Result<bool>.Failure(Error.Failure("System.GovDown", "Government system unreachable."));
            }
        }

        private async Task<Result<T>> HandleErrorResponse<T>(HttpResponseMessage response, CancellationToken ct)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            var problem = JsonSerializer.Deserialize<ProblemDetails>(content, _jsonOptions);
            var detail = problem?.Detail ?? "External error";

            return (int)response.StatusCode switch
            {
                404 => Result<T>.Failure(Error.NotFound("Gov.NotFound", detail)),
                400 => Result<T>.Failure(Error.Validation("Gov.BadRequest", detail)),
                401 => Result<T>.Failure(Error.Failure("Gov.Unauthorized", "System unauthorized.")),
                _ => Result<T>.Failure(Error.Failure("Gov.Error", detail))
            };
        }
    }
