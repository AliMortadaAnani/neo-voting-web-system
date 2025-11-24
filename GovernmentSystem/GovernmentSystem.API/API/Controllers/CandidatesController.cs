using GovernmentSystem.API.API.Controllers;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.Controllers
{
    [Route("api/candidates")]
    [Authorize(Roles = "Admin")]
    public class CandidatesController : ApiController
    {
        private readonly ICandidateServices _candidateServices;

        public CandidatesController(ICandidateServices candidateServices)
        {
            _candidateServices = candidateServices;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(List<CandidateResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _candidateServices.GetAllCandidatesAsync();
            return HandleResult(result);
        }

        // POST api/candidates/details
        [HttpPost("details")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetCandidateRequestDTO request)
        {
            var result = await _candidateServices.GetByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] CreateCandidateRequestDTO request)
        {
            var result = await _candidateServices.AddCandidateAsync(request);
            return HandleResult(result);
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCandidateRequestDTO request)
        {
            var result = await _candidateServices.UpdateByNationalIdAsync(request);
            return HandleResult(result);
        }

        // POST api/candidates/delete
        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteCandidateRequestDTO request)
        {
            var result = await _candidateServices.DeleteByNationalIdAsync(request);
            return HandleResult(result);
        }

        [HttpPost("generate-token")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateNewTokenCandidateRequestDTO request)
        {
            var result = await _candidateServices.GenerateNewTokenByNationalIdAsync(request);
            return HandleResult(result);
        }
    }
}