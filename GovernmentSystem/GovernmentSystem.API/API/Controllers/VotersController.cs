using GovernmentSystem.API.API.Controllers;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.Controllers
{

    [Authorize(Roles = "Admin")]
    public class VotersController : ApiController
    {
        private readonly IVoterServices _voterServices;

        public VotersController(IVoterServices voterServices)
        {
            _voterServices = voterServices;
        }

        // GET api/voters/all
        // (Safe to keep as GET because it exposes no IDs in the URL)
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<VoterResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _voterServices.GetAllVotersAsync();
            return HandleResult(result);
        }

        // POST api/voters/details (Was GET /{id})
        // Security: ID is hidden in JSON Body
        [HttpPost("details")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNationalId([FromBody] GetVoterRequestDTO request)
        {
            var result = await _voterServices.GetByNationalIdAsync(request);
            return HandleResult(result);
        }

        // POST api/voters/add
        [HttpPost("add")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] CreateVoterRequestDTO request)
        {
            var result = await _voterServices.AddVoterAsync(request);
            return HandleResult(result);
        }

        // PUT api/voters/update
        // (Already secure, ID is in Body)
        [HttpPut("update")]
        [ProducesResponseType(typeof(VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateVoterRequestDTO request)
        {
            var result = await _voterServices.UpdateByNationalIdAsync(request);
            return HandleResult(result);
        }

        // POST api/voters/delete (Was DELETE /{id})
        // Security: ID is hidden in JSON Body
        [HttpPost("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteVoterRequestDTO request)
        {
            var result = await _voterServices.DeleteByNationalIdAsync(request);
            return HandleResult(result);
        }

        // POST api/voters/generate-token
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