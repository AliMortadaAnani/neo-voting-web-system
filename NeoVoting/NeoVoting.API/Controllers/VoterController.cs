using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.API.Controllers
{
    [EnableRateLimiting("GeneralApiLimiter")]
    [Authorize(Roles = nameof(RoleTypesEnum.Voter))]
    public class VoterController : ApiController
    {
        private readonly IVoterServices _voterServices;
        private readonly ILogger<VoterController> _logger;

        public VoterController(IVoterServices voterServices, ILogger<VoterController> logger)
        {
            _voterServices = voterServices;
            _logger = logger;
        }

        

        [HttpPost("elections/{electionId}/vote")]
        [ProducesResponseType(typeof(ElectionVoteLog_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CastVoteInElection([FromRoute] int electionId, [FromBody] NeoVoting.Application.RequestDTOs.VoterDTOs.Voter_Cast_In_Election_RequestDTO request)
        {
            _logger.LogInformation("Cast vote in election {ElectionId}", electionId);
            var result = await _voterServices.CastVoteInElectionAsync(electionId, request);
            return HandleResult(result);
        }

        [HttpPost("polls/{pollId}/vote")]
        [ProducesResponseType(typeof(PollVoteLog_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CastVoteInPoll([FromRoute] int pollId, [FromBody] NeoVoting.Application.RequestDTOs.VoterDTOs.Voter_Cast_In_Poll_RequestDTO request)
        {
            _logger.LogInformation("Cast vote in poll {PollId}", pollId);
            var result = await _voterServices.CastVoteInPollAsync(pollId, request);
            return HandleResult(result);
        }
    }
}