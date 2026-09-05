using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.API.Controllers
{
    [EnableRateLimiting("GeneralApiLimiter")]
    [AllowAnonymous]
    public class GeneralController : ApiController
    {
        private readonly ILogger<GeneralController> _logger;
        private readonly IGeneralServices _generalServices;

        public GeneralController(IGeneralServices generalServices, ILogger<GeneralController> logger)
        {
            _generalServices = generalServices;
            _logger = logger;
        }

        [HttpGet("elections/active")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveElection()
        {
            _logger.LogInformation("Get active election requested");
            var result = await _generalServices.GetActiveElectionAsync();
            return HandleResult(result);
        }

        [HttpGet("polls/active")]
        [ProducesResponseType(typeof(Poll_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActivePoll()
        {
            _logger.LogInformation("Get active poll requested");
            var result = await _generalServices.GetActivePollAsync();
            return HandleResult(result);
        }

        [HttpGet("elections/{electionId}")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetElectionById([FromRoute] int electionId)
        {
            _logger.LogInformation("Get election by id requested - {ElectionId}", electionId);
            var result = await _generalServices.GetElectionByIdAsync(electionId);
            return HandleResult(result);
        }

        [HttpGet("polls/{pollId}")]
        [ProducesResponseType(typeof(Poll_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPollById([FromRoute] int pollId)
        {
            _logger.LogInformation("Get poll by id requested - {PollId}", pollId);
            var result = await _generalServices.GetPollByIdAsync(pollId);
            return HandleResult(result);
        }

        [HttpGet("elections/paged")]
        [ProducesResponseType(typeof(List<Election_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedElections([FromQuery] int? page = 1, [FromQuery] int? pageSize = 10)
        {
            _logger.LogInformation("Get paged elections requested - Page: {Page}, Size: {Size}", page, pageSize);
            var paged = await _generalServices.GetPagedElectionsAsync(page, pageSize);
            return Ok(paged);
        }

        [HttpGet("polls/paged")]
        [ProducesResponseType(typeof(List<Poll_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedPolls([FromQuery] int? page = 1, [FromQuery] int? pageSize = 10)
        {
            _logger.LogInformation("Get paged polls requested - Page: {Page}, Size: {Size}", page, pageSize);
            var paged = await _generalServices.GetPagedPollsAsync(page, pageSize);
            return Ok(paged);
        }

        [HttpGet("elections/{electionId}/voteLogs")]
        [ProducesResponseType(typeof(List<ElectionVoteLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedElectionVoteLogs([FromRoute] int electionId, [FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10)
        {
            _logger.LogInformation("Get paged election vote logs requested for election {ElectionId}", electionId);
            var paged = await _generalServices.GetPagedElectionVoteLogsAsync(electionId, pageNumber, pageSize);
            return Ok(paged);
        }

        [HttpGet("polls/{pollId}/voteLogs")]
        [ProducesResponseType(typeof(List<PollVoteLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedPollVoteLogs([FromRoute] int pollId, [FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10)
        {
            _logger.LogInformation("Get paged poll vote logs requested for poll {PollId}", pollId);
            var paged = await _generalServices.GetPagedPollVoteLogsAsync(pollId, pageNumber, pageSize);
            return Ok(paged);
        }

        [HttpGet("polls/{pollId}/results")]
        [ProducesResponseType(typeof(Poll_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPollResults([FromRoute] int pollId)
        {
            _logger.LogInformation("Get poll results requested for poll {PollId}", pollId);
            var result = await _generalServices.GetPollResultsAsync(pollId);
            return HandleResult(result);
        }

        [HttpGet("elections/{electionId}/candidateResults")]
        [ProducesResponseType(typeof(List<CandidateProfile_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedCandidateResultsForElection([FromRoute] int electionId, [FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10)
        {
            _logger.LogInformation("Get paged candidate results for election {ElectionId}", electionId);
            var paged = await _generalServices.GetPagedCandidateResultsForElectionAsync(electionId, pageNumber, pageSize);
            return Ok(paged);
        }
    }
}