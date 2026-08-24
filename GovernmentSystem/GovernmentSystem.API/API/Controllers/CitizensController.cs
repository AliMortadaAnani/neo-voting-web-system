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

        public CitizensController(ICitizenServices citizenServices)
        {
            _citizenServices = citizenServices;
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<PagedResult<CitizenResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _citizenServices.GetCitizensPagedAsync(pageNumber, pageSize);
            return HandleResult(result);
        }

        [HttpPost("details")]
        [ProducesResponseType(typeof(CitizenResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetCitizenRequestDTO request)
        {
            var result = await _citizenServices.GetCitizenByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(CitizenResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Add([FromBody] CreateCitizenRequestDTO request)
        {
            var result = await _citizenServices.AddCitizenAsync(request);
            return HandleResult(result, true);
        }

        [HttpPut("updateDetails")]
        [ProducesResponseType(typeof(CitizenResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCitizenRequestDTO request)
        {
            var result = await _citizenServices.UpdateCitizenByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteCitizenRequestDTO request)
        {
            var result = await _citizenServices.DeleteCitizenByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpGet("totalCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalCount()
        {
            var result = await _citizenServices.GetCitizensTotalCountAsync();
            return HandleResult(result);
        }
    }
}