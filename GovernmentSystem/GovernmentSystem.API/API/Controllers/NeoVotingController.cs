using GovernmentSystem.API.API.Controllers;
using GovernmentSystem.API.API.Filters; // Ensure this points to where ApiKeyAuthAttribute is
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.Controllers
{
    // Route must match your IP Whitelist logic (/api/external)
    [Route("api/external")]
    [ApiKeyAuth] // 🔒 Secured by API Key
    public class NeoVotingController : ApiController
    {
        private readonly IVoterServices _voterServices;
        private readonly ICandidateServices _candidateServices;

        public NeoVotingController(IVoterServices voterServices, ICandidateServices candidateServices)
        {
            _voterServices = voterServices;
            _candidateServices = candidateServices;
        }

        // --- VOTER INTEGRATION ---

        // POST api/external/voters/verify
        // Used by Neo-Voting to check if a user can login
        [HttpPost("voters/verify")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)] // Not Found
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyVoter([FromBody] NeoVoting_GetVoterRequestDTO request)
        {
            var result = await _voterServices.GetVoterForNeoVotingAsync(request);
            return HandleResult(result);
        }

        // PUT api/external/voters/register
        // Used when a voter creates an account on Neo-Voting
        [HttpPut("voters/registered-in-neovoting")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterVoter([FromBody] NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            var result = await _voterServices.UpdateVoterIsRegisteredToTrueAsync(request);
            return HandleResult(result);
        }

        // PUT api/external/voters/vote-status
        // Used when a voter successfully casts a vote
        [HttpPut("voters/mark-as-voted")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateVoteStatus([FromBody] NeoVoting_VoterHasVotedRequestDTO request)
        {
            var result = await _voterServices.UpdateHasVotedToTrueAsync(request);
            return HandleResult(result);
        }

        // --- CANDIDATE INTEGRATION ---

        // POST api/external/candidates/verify
        // Used by Candidates to login to their dashboard
        [HttpPost("candidates/verify")]
        [ProducesResponseType(typeof(NeoVoting_CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyCandidate([FromBody] NeoVoting_GetCandidateRequestDTO request)
        {
            var result = await _candidateServices.GetCandidateForNeoVotingAsync(request);
            return HandleResult(result);
        }

        // PUT api/external/candidates/register
        // Used when a candidate activates their profile
        [HttpPut("candidates/registered-in-neovoting")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterCandidate([FromBody] NeoVoting_CandidateIsRegisteredRequestDTO request)
        {
            var result = await _candidateServices.UpdateCandidateIsRegisteredToTrueAsync(request);
            return HandleResult(result);
        }

        // --- SYSTEM UTILITIES ---

        // POST api/external/reset-election
        // Used to reset the simulation
        [HttpPost("reset-vote-status")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetElection()
        {
            var result = await _voterServices.ResetAllVotedAsFalseAsync();
            return HandleResult(result);
        }
    }
}