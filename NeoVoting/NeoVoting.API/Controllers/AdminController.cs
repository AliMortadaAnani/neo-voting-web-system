using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.API.Controllers
{
    /// <summary>
    /// Administrative operations for managing elections and viewing system audit logs.
    /// </summary>
    ///

    [EnableRateLimiting("GeneralApiLimiter")]
    [Authorize(Roles = nameof(RoleTypesEnum.Admin))]
    public class AdminController : ApiController
    {
        private readonly IAdminServices _adminServices;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminServices adminServices, ILogger<AdminController> logger)
        {
            _adminServices = adminServices;
            _logger = logger;
        }

        [HttpPost("elections")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateElection([FromBody] NeoVoting.Application.RequestDTOs.AdminDTOs.ElectionCreate_RequestDTO request)
        {
            _logger.LogInformation("Create election requested");
            var result = await _adminServices.CreateElectionAsync(request);
            return HandleResult(result, result.IsSuccess);
        }

        [HttpPost("polls")]
        [ProducesResponseType(typeof(Poll_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePoll([FromBody] NeoVoting.Application.RequestDTOs.AdminDTOs.PollCreate_RequestDTO request)
        {
            _logger.LogInformation("Create poll requested");
            var result = await _adminServices.CreatePollAsync(request);
            return HandleResult(result, result.IsSuccess);
        }

        [HttpPost("elections/{electionId}/start")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartElection([FromRoute] int electionId)
        {
            _logger.LogInformation("Start election requested - {ElectionId}", electionId);
            var result = await _adminServices.StartElectionAsync(electionId);
            return HandleResult(result);
        }

        [HttpPost("elections/{electionId}/complete")]
        [ProducesResponseType(typeof(Election_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteElection([FromRoute] int electionId)
        {
            _logger.LogInformation("Complete election requested - {ElectionId}", electionId);
            var result = await _adminServices.CompleteElectionAsync(electionId);
            return HandleResult(result);
        }

        [HttpPost("polls/{pollId}/start")]
        [ProducesResponseType(typeof(Poll_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartPoll([FromRoute] int pollId)
        {
            _logger.LogInformation("Start poll requested - {PollId}", pollId);
            var result = await _adminServices.StartPollAsync(pollId);
            return HandleResult(result);
        }

        [HttpPost("polls/{pollId}/complete")]
        [ProducesResponseType(typeof(Poll_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompletePoll([FromRoute] int pollId)
        {
            _logger.LogInformation("Complete poll requested - {PollId}", pollId);
            var result = await _adminServices.CompletePollAsync(pollId);
            return HandleResult(result);
        }

        [HttpGet("systemAuditLogs")]
        [ProducesResponseType(typeof(List<SystemAuditLog_ResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedSystemAuditLogs([FromQuery] NeoVoting.Domain.Enums.SystemActionTypesEnum? actionTypesEnum, [FromQuery] int? adminId, [FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10)
        {
            _logger.LogInformation("Get paged system audit logs requested");
            var paged = await _adminServices.GetPagedSystemAuditLogsAsync(actionTypesEnum, adminId, pageNumber, pageSize);
            return Ok(paged);
        }
    }
}