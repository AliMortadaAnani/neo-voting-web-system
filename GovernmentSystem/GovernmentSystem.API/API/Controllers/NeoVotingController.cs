using GovernmentSystem.API.API.Controllers;
using GovernmentSystem.API.API.Filters;
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
        [HttpPost("voters/verify")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyVoter([FromBody] NeoVoting_GetVoterRequestDTO request)
        {
            var result = await _voterServices.GetVoterForNeoVotingAsync(request);
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
        [HttpPut("voters/registered-in-neovoting")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterVoter([FromBody] NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            var result = await _voterServices.UpdateVoterIsRegisteredToTrueAsync(request);
            return HandleResult(result);
        }


        /// <summary>
        /// Marks a voter as non registered in the Neo-Voting system(fallback if neo voting system registration went wrong).
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
        [HttpPut("voters/un-registered-in-neovoting")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnRegisterVoter([FromBody] NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            var result = await _voterServices.UpdateVoterIsRegisteredToFalseAsync(request);
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
        [HttpPut("voters/mark-as-voted")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateVoteStatus([FromBody] NeoVoting_VoterHasVotedRequestDTO request)
        {
            var result = await _voterServices.UpdateHasVotedToTrueAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Records that a voter has not cast their vote.(fallback if neo voting system casting vote went wrong).
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
        [HttpPut("voters/mark-as-non-voted")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateVoteStatusToFalse([FromBody] NeoVoting_VoterHasVotedRequestDTO request)
        {
            var result = await _voterServices.UpdateHasVotedToFalseAsync(request);
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
        [HttpPut("candidates/registered-in-neovoting")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterCandidate([FromBody] NeoVoting_CandidateIsRegisteredRequestDTO request)
        {
            var result = await _candidateServices.UpdateCandidateIsRegisteredToTrueAsync(request);
            return HandleResult(result);
        }


        /// <summary>
        /// Marks a candidate as non registered in the Neo-Voting system(fallback if neo voting system registration went wrong).
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
        [HttpPut("candidates/un-registered-in-neovoting")]
        [ProducesResponseType(typeof(NeoVoting_VoterResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnRegisterCandidate([FromBody] NeoVoting_CandidateIsRegisteredRequestDTO request)
        {
            var result = await _candidateServices.UpdateCandidateIsRegisteredToFalseAsync(request);
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
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetElection()
        {
            var result = await _voterServices.ResetAllVotedAsFalseAsync();
            return HandleResult(result);
        }
    }
}