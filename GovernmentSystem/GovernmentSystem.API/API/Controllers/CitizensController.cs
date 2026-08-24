using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GovernmentSystem.API.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("GeneralApiLimiter")]
    public class CitizensController : ApiController
    {
        private readonly ICitizenServices _citizenServices;
        private readonly ILogger<CitizensController> _logger;

        public CitizensController(ICitizenServices citizenServices, ILogger<CitizensController> logger)
        {
            _citizenServices = citizenServices;
            _logger = logger;
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<PagedResult<CitizenResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GetPaged citizens requested - PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            var result = await _citizenServices.GetCitizensPagedAsync(pageNumber, pageSize);
            if (result.IsSuccess)
                _logger.LogInformation("GetPaged citizens successful");
            else
                _logger.LogWarning("GetPaged citizens failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("details")]
        [ProducesResponseType(typeof(CitizenResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetCitizenRequestDTO request)
        {
            _logger.LogInformation("Get citizen details requested");
            var result = await _citizenServices.GetCitizenByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Get citizen details successful");
            else
                _logger.LogWarning("Get citizen details failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(CitizenResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Add([FromBody] CreateCitizenRequestDTO request)
        {
            _logger.LogInformation("Add citizen requested");
            var result = await _citizenServices.AddCitizenAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Add citizen successful");
            else
                _logger.LogWarning("Add citizen failed: {Error}", result.Error.Description);
            return HandleResult(result, true);
        }

        [HttpPut("updateDetails")]
        [ProducesResponseType(typeof(CitizenResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCitizenRequestDTO request)
        {
            _logger.LogInformation("Update citizen details requested");
            var result = await _citizenServices.UpdateCitizenByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Update citizen details successful");
            else
                _logger.LogWarning("Update citizen details failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteCitizenRequestDTO request)
        {
            _logger.LogInformation("Delete citizen requested");
            var result = await _citizenServices.DeleteCitizenByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Delete citizen successful");
            else
                _logger.LogWarning("Delete citizen failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpGet("totalCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalCount()
        {
            _logger.LogInformation("Get citizens total count requested");
            var result = await _citizenServices.GetCitizensTotalCountAsync();
            if (result.IsSuccess)
                _logger.LogInformation("Get citizens total count successful - Count: {Count}", result.Value);
            else
                _logger.LogWarning("Get citizens total count failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }
    }
}