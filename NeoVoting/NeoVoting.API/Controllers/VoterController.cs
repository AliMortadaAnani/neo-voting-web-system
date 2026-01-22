using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;

namespace NeoVoting.API.Controllers
{
    /// <summary>
    /// Voter operations for casting votes and viewing election data.
    /// </summary>
    [Authorize(Roles = nameof(RoleTypesEnum.Voter))]
    public class VoterController : ApiController
    {
        private readonly IVoterServices _voterServices;

        public VoterController(IVoterServices voterServices)
        {
            _voterServices = voterServices;
        }

        /// <summary>
        /// Casts a vote for the specified election.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="request">The vote request containing selected candidate IDs and voter credentials.</param>
        /// <remarks>
        /// **Rules:**
        /// - Exactly 5 unique candidate profile IDs must be selected.
        /// - Requires valid National ID, Voting Token, and Password for verification.
        /// - Voter must not have already voted in this election.
        /// - Election must be in the Voting phase.
        /// - Returns 409 if voter has already voted.
        /// </remarks>
        [HttpPost("elections/{electionId:guid}/vote")]
        [ProducesResponseType(typeof(VoterCastVote_ResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails409ErrorTypes), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails500ErrorTypes), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CastVote([FromRoute] Guid electionId, [FromBody] VoterCastVote_RequestDTO request, CancellationToken ct)
        {
            var result = await _voterServices.VoterCastVoteAsync(electionId, request, ct);
            return HandleResult(result, Created: true);
        }

        /// <summary>
        /// Retrieves a paginated list of candidates for a specific election.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of candidates per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns 404 if election does not exist or page has no candidates.
        /// - Returns 400 for invalid page parameters.
        /// </remarks>
        [HttpGet("elections/{electionId:guid}/candidates/paged")]
        [ProducesResponseType(typeof(IReadOnlyList<CandidateProfile_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails500ErrorTypes), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedCandidates(
            [FromRoute] Guid electionId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _voterServices.GetPagedCandidatesByElectionIdAsync(electionId, pageNumber, pageSize, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a paginated list of candidates for a specific election filtered by governorate.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="governorateId">The governorate ID to filter candidates by (1-5).</param>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of candidates per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns 404 if election does not exist or page has no candidates.
        /// - Returns 400 for invalid page parameters or governorate ID.
        /// </remarks>
        [HttpGet("elections/{electionId:guid}/candidates/by-governorate/{governorateId:int}/paged")]
        [ProducesResponseType(typeof(IReadOnlyList<CandidateProfile_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails500ErrorTypes), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedCandidatesByGovernorate(
            [FromRoute] Guid electionId,
            [FromRoute] int governorateId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _voterServices.GetPagedCandidatesByElectionIdAndGovernorateIdAsync(electionId, governorateId, pageNumber, pageSize, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a specific public vote log by vote ID.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="voteId">The unique identifier of the vote.</param>
        /// <remarks>
        /// **Notes:**
        /// - Allows voters to verify their vote was recorded.
        /// - Returns 404 if vote log does not exist.
        /// </remarks>
        [HttpGet("elections/{electionId:guid}/vote-logs/{voteId:guid}")]
        [ProducesResponseType(typeof(PublicVoteLog_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails500ErrorTypes), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVoteLogByVoteId([FromRoute] Guid electionId, [FromRoute] Guid voteId, CancellationToken ct)
        {
            var result = await _voterServices.GetPublicVoteLogByVoteIdAsync(electionId, voteId, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a paginated list of public vote logs for a specific election.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of logs per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns 404 if election does not exist or page has no logs.
        /// - Returns 400 for invalid page parameters.
        /// </remarks>
        [HttpGet("elections/{electionId:guid}/vote-logs/paged")]
        [ProducesResponseType(typeof(IReadOnlyList<PublicVoteLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedVoteLogs(
            [FromRoute] Guid electionId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _voterServices.GetPagedPublicVoteLogsByElectionIdAsync(electionId, pageNumber, pageSize, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a paginated list of public vote logs for a specific election filtered by governorate.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="governorateId">The governorate ID to filter logs by (1-5).</param>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of logs per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns 404 if election does not exist or page has no logs.
        /// - Returns 400 for invalid page parameters or governorate ID.
        /// </remarks>
        [HttpGet("elections/{electionId:guid}/vote-logs/by-governorate/{governorateId:int}/paged")]
        [ProducesResponseType(typeof(IReadOnlyList<PublicVoteLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedVoteLogsByGovernorate(
            [FromRoute] Guid electionId,
            [FromRoute] int governorateId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _voterServices.GetPagedPublicVoteLogsByElectionIdAndGovernorateIdAsync(electionId, governorateId, pageNumber, pageSize, ct);
            return HandleResult(result);
        }
    }
}
