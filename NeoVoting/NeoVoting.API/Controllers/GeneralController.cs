using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.ErrorHandling;

namespace NeoVoting.API.Controllers
{
    /// <summary>
    /// Public endpoints for viewing election information and statistics.
    /// </summary>
    [AllowAnonymous]
    public class GeneralController : ApiController
    {
        private readonly IGeneralServices _generalServices;

        public GeneralController(IGeneralServices generalServices)
        {
            _generalServices = generalServices;
        }

        /// <summary>
        /// Retrieves a list of all elections in the system.
        /// </summary>
        /// <remarks>
        /// **Notes:**
        /// - Returns elections ordered by voting end date (most recent first).
        /// - Returns 404 if no elections exist.
        /// </remarks>
        [HttpGet("elections/all")]
        [ProducesResponseType(typeof(IReadOnlyList<Election_ResponseDTO>), StatusCodes.Status200OK)]
        
        public async Task<IActionResult> GetAllElections(CancellationToken ct)
        {
            var result = await _generalServices.GetAllElectionsAsync(ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a list of all completed elections.
        /// </summary>
        /// <remarks>
        /// **Notes:**
        /// - Returns only elections with status "Completed".
        /// - Returns 404 if no completed elections exist.
        /// </remarks>
        [HttpGet("elections/completed")]
        [ProducesResponseType(typeof(IReadOnlyList<Election_ResponseDTO>), StatusCodes.Status200OK)]
        
        public async Task<IActionResult> GetCompletedElections(CancellationToken ct)
        {
            var result = await _generalServices.GetAllCompletedElectionsAsync(ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves the current active (non-completed) election.
        /// </summary>
        /// <remarks>
        /// **Notes:**
        /// - Returns the election that is Upcoming, in Nomination, PreVoting, or Voting phase.
        /// - Returns 404 if no active election exists.
        /// </remarks>
        [HttpGet("elections/active")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentActiveElection(CancellationToken ct)
        {
            var result = await _generalServices.GetCurrentActiveElectionAsync(ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves statistics for a completed election.
        /// </summary>
        /// <param name="electionId">The unique identifier of the completed election.</param>
        /// <remarks>
        /// **Statistics Include:**
        /// - Total registered voters and actual voters.
        /// - Participation percentages (overall, by gender, by age group).
        /// - Candidate nomination count and winning candidates.
        /// - Election dates and parliamentary term dates.
        /// - Returns 404 if election does not exist or is not completed.
        /// </remarks>
        // URL becomes: GET /api/elections/stats?electionId=d290f1ee-6c54-4b01-90e6-d701748f0851
        [HttpGet("elections/completed/stats")]
        [ProducesResponseType(typeof(ElectionCompletedStatistics_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompletedElectionStats(
            [FromQuery] Guid electionId, // Changed to FromQuery
            CancellationToken ct)
        {
            var result = await _generalServices.GetCompletedElectionStatsByIdAsync(electionId, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves statistics for a completed election filtered by governorate.
        /// </summary>
        /// <param name="electionId">The unique identifier of the completed election.</param>
        /// <param name="governorateId">The governorate ID to filter statistics by (1-5).</param>
        /// <remarks>
        /// **Statistics Include:**
        /// - Registered voters and actual voters for the governorate.
        /// - Participation percentages for the governorate.
        /// - Candidates and winners from the governorate.
        /// - Returns 404 if election does not exist or is not completed.
        /// - Returns 400 for invalid governorate ID.
        /// </remarks>
        [HttpGet("elections/completed/stats/by-governorate")]
        [ProducesResponseType(typeof(ElectionCompletedStatistics_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompletedElectionStatsByGovernorate(
    [FromQuery] Guid electionId,
    [FromQuery] int governorateId,
    CancellationToken ct)
        {
            var result = await _generalServices.GetCompletedElectionStatsByIdPerGovernorateIdAsync(electionId, governorateId, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves statistics for the current active election.
        /// </summary>
        /// <remarks>
        /// **Statistics Include:**
        /// - Total registered voters (by gender, by age group).
        /// - Registration percentages.
        /// - Number of nominated candidates.
        /// - Election dates and parliamentary term dates.
        /// - Returns 404 if no active election exists.
        /// </remarks>
        [HttpGet("elections/active/stats")]
        [ProducesResponseType(typeof(ElectionCurrentActiveStatistics_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentActiveElectionStats(CancellationToken ct)
        {
            var result = await _generalServices.GetCurrentActiveElectionStatsAsync(ct);
            return HandleResult(result);
        }

        [HttpGet("elections/active/stats/by-governorate")]
        [ProducesResponseType(typeof(ElectionCurrentActiveStatistics_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentActiveElectionStats(
             [FromQuery] int governorateId,
            CancellationToken ct)
        {
            var result = await _generalServices.GetCurrentActiveElectionStatsPerGovernorateIdAsync(governorateId,ct);
            return HandleResult(result);
        }

    }
}
