using GovernmentSystem.API.API.Filters;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GovernmentSystem.API.API.Controllers
{
    // Route must match your IP Whitelist logic (/api/public)
    [Route("api/public")]
    [ApiKeyAuth] //  Secured by API Key
    [EnableRateLimiting("ApiKeyLimiter")]
    public class NeoVotingController : ApiController
    {
        private readonly IVoterServices _voterServices;
        private readonly ICandidateServices _candidateServices;
        private readonly ILogger<NeoVotingController> _logger;

        public NeoVotingController(IVoterServices voterServices, ICandidateServices candidateServices, ILogger<NeoVotingController> logger)
        {
            _voterServices = voterServices;
            _candidateServices = candidateServices;
            _logger = logger;
        }

        [HttpPost("voter/verify")]
        [ProducesResponseType(typeof(VoterVerificationResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyVoter([FromBody] GetVoterVerificationRequestDTO request)
        {
            _logger.LogInformation("Voter verification requested");
            var result = await _voterServices.VerifyVoterCredentialsAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Voter verification successful");
            else
                _logger.LogWarning("Voter verification failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("candidate/verify")]
        [ProducesResponseType(typeof(CandidateVerificationResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyCandidate([FromBody] GetCandidateVerificationRequestDTO request)
        {
            _logger.LogInformation("Candidate verification requested");
            var result = await _candidateServices.VerifyCandidateCredentialsAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Candidate verification successful");
            else
                _logger.LogWarning("Candidate verification failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }
    }
}