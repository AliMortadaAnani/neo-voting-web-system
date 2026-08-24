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

        public CandidatesController(ICandidateServices candidateServices)
        {
            _candidateServices = candidateServices;
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<PagedResult<CandidateResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _candidateServices.GetCandidatesPagedAsync(pageNumber, pageSize);
            return HandleResult(result);
        }

        [HttpPost("details")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetCandidateRequestDTO request)
        {
            var result = await _candidateServices.GetCandidateByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Add([FromBody] CreateCandidateRequestDTO request)
        {
            var result = await _candidateServices.AddCandidateAsync(request);
            return HandleResult(result, true);
        }

        [HttpPut("generateNewToken")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCandidateRequestDTO request)
        {
            var result = await _candidateServices.GenerateNewNominationTokenByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteCandidateRequestDTO request)
        {
            var result = await _candidateServices.DeleteCandidateByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpGet("totalCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalCount()
        {
            var result = await _candidateServices.GetCandidatesTotalCountAsync();
            return HandleResult(result);
        }
    }
}