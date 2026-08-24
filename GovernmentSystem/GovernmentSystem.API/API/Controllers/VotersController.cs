using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GovernmentSystem.API.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("GeneralApiLimiter")]
    public class VotersController : ApiController
    {
        private readonly IVoterServices _voterServices;
        private readonly ILogger<VotersController> _logger;

        public VotersController(IVoterServices voterServices, ILogger<VotersController> logger)
        {
            _voterServices = voterServices;
            _logger = logger;
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<VoterResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GetPaged voters requested - PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            var result = await _voterServices.GetVotersPagedAsync(pageNumber, pageSize);
            if (result.IsSuccess)
                _logger.LogInformation("GetPaged voters successful");
            else
                _logger.LogWarning("GetPaged voters failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("details")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetVoterRequestDTO request)
        {
            _logger.LogInformation("Get voter details requested");
            var result = await _voterServices.GetVoterByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Get voter details successful");
            else
                _logger.LogWarning("Get voter details failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Add([FromBody] CreateVoterRequestDTO request)
        {
            _logger.LogInformation("Add voter requested");
            var result = await _voterServices.AddVoterAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Add voter successful");
            else
                _logger.LogWarning("Add voter failed: {Error}", result.Error.Description);
            return HandleResult(result, true);
        }

        [HttpPut("generateNewToken")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateVoterRequestDTO request)
        {
            _logger.LogInformation("Generate new voting token for voter requested");
            var result = await _voterServices.GenerateNewVotingTokenByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Generate new voting token successful");
            else
                _logger.LogWarning("Generate new voting token failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteVoterRequestDTO request)
        {
            _logger.LogInformation("Delete voter requested");
            var result = await _voterServices.DeleteVoterByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Delete voter successful");
            else
                _logger.LogWarning("Delete voter failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpGet("totalCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalCount()
        {
            _logger.LogInformation("Get voters total count requested");
            var result = await _voterServices.GetVotersTotalCountAsync();
            if (result.IsSuccess)
                _logger.LogInformation("Get voters total count successful - Count: {Count}", result.Value);
            else
                _logger.LogWarning("Get voters total count failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }
    }
}