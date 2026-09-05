using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.API.Controllers
{
    [EnableRateLimiting("GeneralApiLimiter")]
    [Authorize(Roles = nameof(RoleTypesEnum.Candidate))]
    public class CandidateController : ApiController
    {
        private readonly ICandidateServices _candidateServices;
        private readonly ILogger<CandidateController> _logger;

        public CandidateController(ICandidateServices candidateServices, ILogger<CandidateController> logger)
        {
            _candidateServices = candidateServices;
            _logger = logger;
        }

        [HttpPost("elections/{electionId}/profiles")]
        [ProducesResponseType(typeof(CandidateProfile_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCandidateProfile([FromRoute] int electionId, [FromBody] NeoVoting.Application.RequestDTOs.CandidateDTOs.CandidateProfile_Create_RequestDTO request)
        {
            _logger.LogInformation("Create candidate profile requested for election {ElectionId}", electionId);
            var result = await _candidateServices.CreateCandidateProfileAsync(electionId, request);
            return HandleResult(result, result.IsSuccess);
        }
    }
}