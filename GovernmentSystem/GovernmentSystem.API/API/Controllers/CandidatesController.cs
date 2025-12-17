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

        /// <summary>
        /// Retrieves a list of all candidates.
        /// </summary>
        /// <remarks>
        /// **Notes:**
        /// - Returns a full list of candidates in the system(with all db fields).
        /// - Returns 404 if the database is empty.
        /// </remarks>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<CandidateResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _candidateServices.GetAllCandidatesAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a paginated list of candidates.
        /// </summary>
        /// <param name="pageNumber">The current page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of candidates per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns a paginated list of candidates (all db fields).
        /// </remarks>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<CandidateResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _candidateServices.GetPaginatedCandidatesAsync(pageNumber, pageSize);
            return HandleResult(result);
        }

        /// <summary>
        /// Gets a specific candidate's profile(with all db fields)..
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Search is performed using the National ID.
        /// - Returns 404 if no matching candidate is found.
        /// </remarks>
        [HttpPost("details")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetCandidateRequestDTO request)
        {
            var result = await _candidateServices.GetByNationalIdAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Registers a new candidate in the system.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Internal IDs are auto-generated(Id,nationalId,nominationToken).
        /// - Gender must be 'M' or 'F'.
        /// - Date of Birth should be a valid past date.(18 years or older).
        /// - GovernorateId must reference an existing governorate.(1-5)
        /// </remarks>
        [HttpPost("add")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add([FromBody] CreateCandidateRequestDTO request)
        {
            var result = await _candidateServices.AddCandidateAsync(request);
            return HandleResult(result, true);
        }

        /// <summary>
        /// Updates an existing candidate's personal details.
        /// </summary>
        /// <remarks>
        /// **Restrictions:**
        /// - Search is performed using the National ID.
        /// - Id and National ID cannot be changed here.
        /// - Nomination Token cannot be changed here (use Generate Token endpoint).
        /// - Returns 404 if no matching candidate is found.
        /// - Gender must be 'M' or 'F'.
        /// - Date of Birth should be a valid past date.(18 years or older).
        /// - GovernorateId must reference an existing governorate.(1-5)
        /// </remarks>
        [HttpPut("update")]
        [ProducesResponseType(typeof(CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateCandidateRequestDTO request)
        {
            var result = await _candidateServices.UpdateByNationalIdAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Permanently removes a candidate from the system.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires a valid National ID.
        /// - Returns 404 if no matching candidate is found.
        /// </remarks>
        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromBody] DeleteCandidateRequestDTO request)
        {
            var result = await _candidateServices.DeleteByNationalIdAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Generates a new secure access token for a candidate.
        /// </summary>
        /// <remarks>
        /// **Usage:**
        /// - Call this if the user forgot their token or it was compromised.
        /// - The old token will immediately become invalid.
        /// - The new token will be valid.
        /// - Returns 404 if no matching candidate is found.
        /// </remarks>
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