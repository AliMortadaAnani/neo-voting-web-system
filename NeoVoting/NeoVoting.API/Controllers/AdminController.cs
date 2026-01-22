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
    /// Administrative operations for managing elections and viewing system audit logs.
    /// </summary>
    [Authorize(Roles = nameof(RoleTypesEnum.Admin))]
    public class AdminController : ApiController
    {
        private readonly IAdminServices _adminServices;

        public AdminController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

        /// <summary>
        /// Creates a new election in the system.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Election name must be unique.
        /// - Nomination start date must be in the future.
        /// - Dates must be in logical order: NominationStart → NominationEnd → VotingStart → VotingEnd.
        /// - Only one active (non-completed) election can exist at a time.
        /// </remarks>
        [HttpPost("elections/add")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        
        [ProducesResponseType(typeof(ProblemDetails409ErrorTypes), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails500ErrorTypes), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddElection([FromBody] ElectionAdd_RequestDTO request, CancellationToken ct)
        {
            var result = await _adminServices.AddElectionAsync(request, ct);
            return HandleResult(result, Created: true);
        }

        /// <summary>
        /// Retrieves a paginated list of all system audit logs.
        /// </summary>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of logs per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns 404 if the requested page has no logs.
        /// - Returns 400 for invalid page parameters.
        /// </remarks>
        [HttpGet("audit-logs/paged")]
        [ProducesResponseType(typeof(IReadOnlyList<SystemAuditLog_ResponseDTO>), StatusCodes.Status200OK)]

        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedAuditLogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            
            var result = await _adminServices.GetPagedSystemAuditLogsAsync(pageNumber, pageSize, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a paginated list of system audit logs filtered by action type.
        /// </summary>
        /// <param name="actionType">The type of system action to filter by.</param>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of logs per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Valid action types: VOTER_REGISTERED, CANDIDATE_REGISTERED, CANDIDATE_PROFILE_CREATED, etc.
        /// - Returns 404 if the requested page has no logs.
        /// - Returns 400 for invalid page parameters or action type.
        /// </remarks>
        [HttpGet("audit-logs/by-action-type")]
        [ProducesResponseType(typeof(IReadOnlyList<SystemAuditLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedAuditLogsByActionType(
            [FromQuery] SystemActionTypesEnum actionType,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            
            var result = await _adminServices.GetPagedSystemAuditLogsByActionTypeAsync(actionType, pageNumber, pageSize, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a paginated list of system audit logs filtered by election ID.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of logs per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns 404 if the requested page has no logs or election does not exist.
        /// - Returns 400 for invalid page parameters.
        /// </remarks>
        [HttpGet("audit-logs/by-election")]
        [ProducesResponseType(typeof(IReadOnlyList<SystemAuditLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPagedAuditLogsByElectionId(
            [FromQuery] Guid electionId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
           
            var result = await _adminServices.GetPagedSystemAuditLogsByElectionIdAsync(electionId, pageNumber, pageSize, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves all system audit logs for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns all logs associated with the specified user (not paginated).
        /// - Returns 404 if no logs found for the user.
        /// </remarks>
        [HttpGet("audit-logs/by-user")]
        [ProducesResponseType(typeof(IReadOnlyList<SystemAuditLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails400ErrorTypes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails401ErrorTypes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails403ErrorTypes), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails404ErrorTypes), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuditLogsByUserId(
            
           [FromQuery] Guid userId, CancellationToken ct)
        {
            var result = await _adminServices.GetSystemAuditLogsByUserIdAsync(userId, ct);
            return HandleResult(result);
        }
    }
}
