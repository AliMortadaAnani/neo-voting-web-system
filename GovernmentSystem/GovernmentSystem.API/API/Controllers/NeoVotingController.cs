using GovernmentSystem.API.API.Controllers;
using GovernmentSystem.API.API.Filters;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentSystem.API.API.Controllers
{
    // Route must match your IP Whitelist logic (/api/external)
    [Route("api/external")]
    [ApiKeyAuth] //  Secured by API Key
    public class NeoVotingController(INeoVotingServices neoVotingServices) : ApiController
    {
        private readonly INeoVotingServices _neoVotingServices = neoVotingServices;

        // --- VOTER INTEGRATION ---

        /// <summary>
        /// Verifies a voter's credentials and eligibility(Required to proceed critical actions in Neo-Voting System : Registration,Reset password,Casting a Vote).
        /// </summary>
        /// <remarks>
        /// **Validation Rules:**
        /// - Requires both valid National ID and voting Token.
        /// - Returns 404 if National ID does not exist.
        /// - Returns 401(Invalid) if user exists but is not eligible for election.
        /// - Returns 401(Invalid) if user exists but it's voting token is invalid.
        /// - Requires valid API Key for authorization.
        /// - Requested from allowed IPs only
        /// </remarks>
        [HttpPost("voters/get")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyVoter([FromBody] NeoVoting_GetVoterRequestDTO request)
        {
            var result = await _neoVotingServices.GetVoterForNeoVotingAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Marks a voter as registered in the Neo-Voting system(Required to proceed critical actions in Neo-Voting System : Reset password,Casting a Vote).
        /// </summary>
        /// <remarks>
        /// **Validation Rules:**
        /// - Requires both valid National ID and voting Token.
        /// - Returns 404 if National ID does not exist.
        /// - Returns 401(Invalid) if user exists but is not eligible for election.
        /// - Returns 401(Invalid) if user exists but it's voting token is invalid.
        /// - Requires valid API Key for authorization.
        /// - Requested from allowed IPs only
        /// </remarks>
        [HttpPut("voters/register")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterVoter([FromBody] NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            var result = await _neoVotingServices.UpdateVoterIsRegisteredToTrueAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Records that a voter has successfully cast their vote.
        /// </summary>
        /// <remarks>
        /// **Validation Rules:**
        /// - Requires both valid National ID and voting Token.
        /// - Returns 404 if National ID does not exist.
        /// - Returns 401(Invalid) if user exists but is not eligible for election.
        /// - Returns 401(Invalid) if user exists but it's voting token is invalid.
        /// - Returns 401(Invalid) if user exists but is not registered in Neo-Voting System.
        /// - Requires valid API Key for authorization.
        /// - Requested from allowed IPs only
        /// </remarks>
        [HttpPut("voters/vote")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateVoteStatus([FromBody] NeoVoting_VoterHasVotedRequestDTO request)
        {
            var result = await _neoVotingServices.UpdateHasVotedToTrueAsync(request);
            return HandleResult(result);
        }

        // --- CANDIDATE INTEGRATION ---

        /// <summary>
        /// Verifies a candidate's credentials and eligibility(Required to proceed critical actions in Neo-Voting System : Registration,Reset password,Creating and Updating Candidate profile).
        /// </summary>
        /// <remarks>
        /// **Validation Rules:**
        /// - Requires both valid National ID and nomination Token.
        /// - Returns 404 if National ID does not exist.
        /// - Returns 401(Invalid) if user exists but is not eligible for election.
        /// - Returns 401(Invalid) if user exists but it's nomination token is invalid.
        /// - Requires valid API Key for authorization.
        /// - Requested from allowed IPs only
        /// </remarks>
        [HttpPost("candidates/get")]
        [ProducesResponseType(typeof(NeoVoting_CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyCandidate([FromBody] NeoVoting_GetCandidateRequestDTO request)
        {
            var result = await _neoVotingServices.GetCandidateForNeoVotingAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Marks a candidate as registered in the Neo-Voting system(Required to proceed critical actions in Neo-Voting System : Reset password,Creating and Updating Candidate profile).
        /// </summary>
        /// <remarks>
        /// **Validation Rules:**
        /// - Requires both valid National ID and nomination Token.
        /// - Returns 404 if National ID does not exist.
        /// - Returns 401(Invalid) if user exists but is not eligible for election.
        /// - Returns 401(Invalid) if user exists but it's nomination token is invalid.
        /// - Requires valid API Key for authorization.
        /// - Requested from allowed IPs only
        /// </remarks>
        [HttpPut("candidates/register")]
        [ProducesResponseType(typeof(NeoVoting_CandidateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound404ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BadRequest400ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Conflict409ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterCandidate([FromBody] NeoVoting_CandidateIsRegisteredRequestDTO request)
        {
            var result = await _neoVotingServices.UpdateCandidateIsRegisteredToTrueAsync(request);
            return HandleResult(result);
        }

        // --- SYSTEM UTILITIES ---

        /// <summary>
        /// Resets the voting status for all users in the system(after completed election and before upcoming election).
        /// </summary>
        /// <remarks>
        /// **Usage:**
        /// - Should be called by NeoVoting Admin before starting a NEW election.
        /// - Sets "HasVoted" to false for every voter.
        /// - Requires valid API Key for authorization.
        /// - Requested from allowed IPs only
        /// </remarks>
        [HttpPost("reset-vote-status")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Unauthorized401ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetElection()
        {
            var result = await _neoVotingServices.ResetAllVotedAsFalseAsync();
            return HandleResult(result);
        }
    }
}