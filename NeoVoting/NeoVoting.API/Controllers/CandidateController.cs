using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Enums;

namespace NeoVoting.API.Controllers
{
    /// <summary>
    /// Candidate operations for managing their election profile.
    /// </summary>
    [Authorize(Roles = nameof(RoleTypesEnum.Candidate))]
    public class CandidateController : ApiController
    {
        private readonly ICandidateServices _candidateServices;

        public CandidateController(ICandidateServices candidateServices)
        {
            _candidateServices = candidateServices;
        }

        /// <summary>
        /// Creates a new candidate profile for the specified election.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="request">The candidate profile details including goals and nomination reasons.</param>
        /// <remarks>
        /// **Rules:**
        /// - Requires valid National ID and Nomination Token for verification.
        /// - Goals and Nomination Reasons are required (max 1000 characters each).
        /// - Election must be in the Nomination phase.
        /// - Candidate can only have one profile per election.
        /// - Returns 409 if profile already exists for this election.
        /// </remarks>
        [HttpPost("elections/{electionId:guid}/profile")]
        [ProducesResponseType(typeof(CandidateProfile_ResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddProfile(Guid electionId, [FromBody] CandidateProfileAdd_RequestDTO request, CancellationToken ct)
        {
            var result = await _candidateServices.AddCandidateProfileByElectionIdAsync(electionId, request, ct);
            return HandleResult(result, Created: true);
        }

        /// <summary>
        /// Updates the current candidate's profile for the specified election.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="request">The updated candidate profile details.</param>
        /// <remarks>
        /// **Rules:**
        /// - Requires valid National ID and Nomination Token for verification.
        /// - Goals and Nomination Reasons are required (max 1000 characters each).
        /// - Election must be in the Nomination phase.
        /// - Returns 404 if profile does not exist.
        /// </remarks>
        [HttpPut("elections/{electionId:guid}/profile")]
        [ProducesResponseType(typeof(CandidateProfile_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile(Guid electionId, [FromBody] CandidateProfileUpdate_RequestDTO request, CancellationToken ct)
        {
            var result = await _candidateServices.UpdateCandidateProfileByElectionIdAsync(electionId, request, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves the current candidate's profile for the specified election.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <remarks>
        /// **Notes:**
        /// - Returns the profile for the authenticated candidate.
        /// - Returns 404 if profile does not exist for this election.
        /// </remarks>
        [HttpGet("elections/{electionId:guid}/profile/me")]
        [ProducesResponseType(typeof(CandidateProfile_ResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(Guid electionId, CancellationToken ct)
        {
            var result = await _candidateServices.GetCandidateProfileByElectionIdAsync(electionId, ct);
            return HandleResult(result);
        }

        /// <summary>
        /// Uploads or updates the profile photo for the current candidate's election profile.
        /// </summary>
        /// <param name="electionId">The unique identifier of the election.</param>
        /// <param name="request">The image file to upload.</param>
        /// <remarks>
        /// **Rules:**
        /// - Allowed formats: JPG, JPEG, PNG, GIF, WEBP.
        /// - Maximum file size: 5 MB.
        /// - Election must be in the Nomination phase.
        /// - Returns the URL/filename of the uploaded photo.
        /// - Returns 404 if profile does not exist.
        /// </remarks>
        [HttpPut("elections/{electionId:guid}/profile/photo")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadProfilePhoto(Guid electionId, [FromForm] CandidateProfileUploadImage_RequestDTO request, CancellationToken ct)
        {
            var result = await _candidateServices.UpdateCandidateProfile_Photo_ByElectionIdAsync(electionId, request, ct);
            return HandleResult(result);
        }
    }
}
