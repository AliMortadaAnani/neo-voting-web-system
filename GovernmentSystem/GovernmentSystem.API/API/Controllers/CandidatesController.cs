using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GovernmentSystem.API.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("GeneralApiLimiter")]
    public class CandidatesController : ApiController
    {
        private readonly ICandidateServices _candidateServices;
        private readonly ILogger<CandidatesController> _logger;

        public CandidatesController(ICandidateServices candidateServices, ILogger<CandidatesController> logger)
        {
            _candidateServices = candidateServices;
            _logger = logger;
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<PagedResult<CandidateResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GetPaged candidates requested - PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            var result = await _candidateServices.GetCandidatesPagedAsync(pageNumber, pageSize);
            if (result.IsSuccess)
                _logger.LogInformation("GetPaged candidates successful");
            else
                _logger.LogWarning("GetPaged candidates failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("details")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetCandidateRequestDTO request)
        {
            _logger.LogInformation("Get candidate details requested");
            var result = await _candidateServices.GetCandidateByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Get candidate details successful");
            else
                _logger.LogWarning("Get candidate details failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Add([FromBody] CreateCandidateRequestDTO request)
        {
            _logger.LogInformation("Add candidate requested");
            var result = await _candidateServices.AddCandidateAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Add candidate successful");
            else
                _logger.LogWarning("Add candidate failed: {Error}", result.Error.Description);
            return HandleResult(result, true);
        }

        [HttpPut("generateNewToken")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCandidateRequestDTO request)
        {
            _logger.LogInformation("Generate new nomination token for candidate requested");
            var result = await _candidateServices.GenerateNewNominationTokenByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Generate new nomination token successful");
            else
                _logger.LogWarning("Generate new nomination token failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteCandidateRequestDTO request)
        {
            _logger.LogInformation("Delete candidate requested");
            var result = await _candidateServices.DeleteCandidateByNationalIdAsync(request);
            if (result.IsSuccess)
                _logger.LogInformation("Delete candidate successful");
            else
                _logger.LogWarning("Delete candidate failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }

        [HttpGet("totalCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalCount()
        {
            _logger.LogInformation("Get candidates total count requested");
            var result = await _candidateServices.GetCandidatesTotalCountAsync();
            if (result.IsSuccess)
                _logger.LogInformation("Get candidates total count successful - Count: {Count}", result.Value);
            else
                _logger.LogWarning("Get candidates total count failed: {Error}", result.Error.Description);
            return HandleResult(result);
        }
    }
}