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

        public NeoVotingController(IVoterServices voterServices, ICandidateServices candidateServices)
        {
            _voterServices = voterServices;
            _candidateServices = candidateServices;
        }

        [HttpPost("voter/verify")]
        [ProducesResponseType(typeof(VoterVerifyResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyVoter([FromBody] GetVoterVerificationRequestDTO request)
        {
            var result = await _voterServices.VerifyVoterCredentialsAsync(request);
            return HandleResult(result);
        }

        [HttpPost("candidate/verify")]
        [ProducesResponseType(typeof(CandidateVerifyResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyCandidate([FromBody] GetCandidateVerificationRequestDTO request)
        {
            var result = await _candidateServices.VerifyCandidateCredentialsAsync(request);
            return HandleResult(result);
        }
    }
}