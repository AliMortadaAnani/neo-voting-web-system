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
    //[EnableRateLimiting("GeneralApiLimiter")]
    [AllowAnonymous]
    public class GeneralController : ApiController
    {
        private readonly ILogger<GeneralController> _logger;
        private readonly IGeneralServices _generalServices;
        private readonly ICandidateServices _candidateServices;
        private readonly IVoterServices _voterServices;
        public GeneralController(IGeneralServices generalServices, IVoterServices voterServices,
            ICandidateServices candidateServices, ILogger<GeneralController> logger)
        {
            _generalServices = generalServices;
            _voterServices = voterServices;
            _candidateServices = candidateServices;
            _logger = logger;
        }

        [HttpGet("elections/{electionId}/profiles/{candidateId}")]
        [ProducesResponseType(typeof(CandidateProfile_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCandidateProfile([FromRoute] int electionId, [FromRoute] int candidateId)
        {
            _logger.LogInformation("Get candidate profile requested for election {ElectionId} and candidate {CandidateId}", electionId, candidateId);
            var result = await _candidateServices.GetCandidateProfileByElectionIdAndCandidateAccountIdAsync(electionId, candidateId);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Candidate profile retrieved successfully for election {ElectionId} and candidate {CandidateId}", electionId, candidateId);
            }
            else
            {
                _logger.LogWarning("Candidate profile retrieval failed for election {ElectionId} and candidate {CandidateId}: {Error}", electionId, candidateId, result.Error?.Description ?? "Unknown error");
            }
            return HandleResult(result);
        }

        [HttpGet("elections/active")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveElection()
        {
            _logger.LogInformation("Get active election requested");
            var result = await _generalServices.GetActiveElectionAsync();
            if (result.IsSuccess)
            {
                _logger.LogInformation("Active election retrieved successfully");
            }
            else
            {
                _logger.LogWarning("Active election retrieval failed: {Error}", result.Error?.Description ?? "Unknown error");
            }
            return HandleResult(result);
        }

        [HttpGet("polls/active")]
        [ProducesResponseType(typeof(Poll_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActivePoll()
        {
            _logger.LogInformation("Get active poll requested");
            var result = await _generalServices.GetActivePollAsync();
            if (result.IsSuccess)
            {
                _logger.LogInformation("Active poll retrieved successfully");
            }
            else
            {
                _logger.LogWarning("Active poll retrieval failed: {Error}", result.Error?.Description ?? "Unknown error");
            }
            return HandleResult(result);
        }

        [HttpGet("elections/paged")]
        [ProducesResponseType(typeof(List<Election_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedElections([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Get paged elections requested - Page: {Page}, Size: {Size}", page, pageSize);
            var paged = await _generalServices.GetPagedElectionsAsync(page, pageSize);
            if (paged.IsSuccess)
            {
                _logger.LogInformation("Paged elections retrieved successfully - Page: {Page}, Size: {Size}", page, pageSize);
            }
            else
            {
                _logger.LogWarning("Paged elections request failed - Page: {Page}, Size: {Size}: {Error}", page, pageSize, paged.Error?.Description ?? "Unknown error");
            }
            return Ok(paged);
        }

        [HttpGet("polls/paged")]
        [ProducesResponseType(typeof(List<Poll_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedPolls([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Get paged polls requested - Page: {Page}, Size: {Size}", page, pageSize);
            var paged = await _generalServices.GetPagedPollsAsync(page, pageSize);
            if (paged.IsSuccess)
            {
                _logger.LogInformation("Paged polls retrieved successfully - Page: {Page}, Size: {Size}", page, pageSize);
            }
            else
            {
                _logger.LogWarning("Paged polls request failed - Page: {Page}, Size: {Size}: {Error}", page, pageSize, paged.Error?.Description ?? "Unknown error");
            }
            return Ok(paged);
        }

        [HttpGet("elections/{electionId}/candidates/paged")]
        [ProducesResponseType(typeof(List<CandidateProfile_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedNominatedCandidates([FromRoute] int electionId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Get paged nominated candidates for election {ElectionId}", electionId);
            var paged = await _voterServices.GetPagedNominatedCandidatesProfilesForElectionAsync(electionId, pageNumber, pageSize);
            if (paged.IsSuccess)
            {
                _logger.LogInformation("Paged nominated candidates retrieved successfully for election {ElectionId}", electionId);
            }
            else
            {
                _logger.LogWarning("Paged nominated candidates request failed for election {ElectionId}: {Error}", electionId, paged.Error?.Description ?? "Unknown error");
            }
            return Ok(paged);
        }

        [HttpGet("elections/{electionId}/voteLogs")]
        [ProducesResponseType(typeof(List<ElectionVoteLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedElectionVoteLogs([FromRoute] int electionId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Get paged election vote logs requested for election {ElectionId}", electionId);
            var paged = await _generalServices.GetPagedElectionVoteLogsAsync(electionId, pageNumber, pageSize);
            if (paged.IsSuccess)
            {
                _logger.LogInformation("Paged election vote logs retrieved successfully for election {ElectionId}", electionId);
            }
            else
            {
                _logger.LogWarning("Paged election vote logs request failed for election {ElectionId}: {Error}", electionId, paged.Error?.Description ?? "Unknown error");
            }
            return Ok(paged);
        }

        [HttpGet("polls/{pollId}/voteLogs")]
        [ProducesResponseType(typeof(List<PollVoteLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedPollVoteLogs([FromRoute] int pollId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Get paged poll vote logs requested for poll {PollId}", pollId);
            var paged = await _generalServices.GetPagedPollVoteLogsAsync(pollId, pageNumber, pageSize);
            if (paged.IsSuccess)
            {
                _logger.LogInformation("Paged poll vote logs retrieved successfully for poll {PollId}", pollId);
            }
            else
            {
                _logger.LogWarning("Paged poll vote logs request failed for poll {PollId}: {Error}", pollId, paged.Error?.Description ?? "Unknown error");
            }
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
            if (result.IsSuccess)
            {
                _logger.LogInformation("Poll results retrieved successfully for poll {PollId}", pollId);
            }
            else
            {
                _logger.LogWarning("Poll results retrieval failed for poll {PollId}: {Error}", pollId, result.Error?.Description ?? "Unknown error");
            }
            return HandleResult(result);
        }

        [HttpGet("elections/{electionId}/candidateResults")]
        [ProducesResponseType(typeof(List<CandidateProfile_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedCandidateResultsForElection([FromRoute] int electionId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Get paged candidate results for election {ElectionId}", electionId);
            var paged = await _generalServices.GetPagedCandidateResultsForElectionAsync(electionId, pageNumber, pageSize);
            if (paged.IsSuccess)
            {
                _logger.LogInformation("Paged candidate results retrieved successfully for election {ElectionId}", electionId);
            }
            else
            {
                _logger.LogWarning("Paged candidate results request failed for election {ElectionId}: {Error}", electionId, paged.Error?.Description ?? "Unknown error");
            }
            return Ok(paged);
        }
    }
}