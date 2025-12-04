using GovernmentSystem.API.API.Controllers;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.Controllers
{
    [Route("api/voters")]
    [Authorize(Roles = "Admin")]
    public class VotersController : ApiController
    {
        private readonly IVoterServices _voterServices;

        public VotersController(IVoterServices voterServices)
        {
            _voterServices = voterServices;
        }

        /// <summary>
        /// Retrieves a list of all voters.
        /// </summary>
        /// <remarks>
        /// **Notes:**
        /// - Returns a full list of voters in the system(with all db fields).
        /// - Returns 404 if the database is empty.
        /// </remarks>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<VoterResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _voterServices.GetAllVotersAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a paginated list of voters.
        /// </summary>
        /// <param name="pageNumber">The current page number (starting from 1).</param>
        /// <param name="pageSize">The maximum number of voters per page.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns a paginated list of voters (all db fields).
        /// </remarks>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<VoterResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _voterServices.GetPaginatedVotersAsync(pageNumber, pageSize);
            return HandleResult(result);
        }

        /// <summary>
        /// Gets a specific voter's profile(with all db fields).
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Search is performed using the National ID.
        /// - Returns 404 if no matching voter is found.
        /// </remarks>
        [HttpPost("details")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetVoterRequestDTO request)
        {
            var result = await _voterServices.GetByNationalIdAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Registers a new voter in the system.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Internal IDs are auto-generated(Id,nationalId,votingToken).
        /// </remarks>
        [HttpPost("add")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add([FromBody] CreateVoterRequestDTO request)
        {
            var result = await _voterServices.AddVoterAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Updates an existing voter's personal details.
        /// </summary>
        /// <remarks>
        /// **Restrictions:**
        /// - Search is performed using the National ID.
        /// - Id and National ID cannot be changed here.
        /// - Voting Token cannot be changed here (use Generate Token endpoint).
        /// - Returns 404 if no matching voter is found.
        /// </remarks>
        [HttpPut("update")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateVoterRequestDTO request)
        {
            var result = await _voterServices.UpdateByNationalIdAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Permanently removes a voter from the system.
        /// </summary>
        /// <remarks>
        /// **Rules:**
        /// - Requires a valid National ID.
        /// - Returns 404 if no matching voter is found.
        /// </remarks>
        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromBody] DeleteVoterRequestDTO request)
        {
            var result = await _voterServices.DeleteByNationalIdAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Generates a new voting token for a voter.
        /// </summary>
        /// <remarks>
        /// **Usage:**
        /// - Call this if the user forgot their token or it was compromised.
        /// - The old token will immediately become invalid.
        /// - The new token will be valid.
        /// - Returns 404 if no matching voter is found.
        /// </remarks>
        [HttpPost("generate-token")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateNewTokenVoterRequestDTO request)
        {
            var result = await _voterServices.GenerateNewTokenByNationalIdAsync(request);
            return HandleResult(result);
        }
    }
}