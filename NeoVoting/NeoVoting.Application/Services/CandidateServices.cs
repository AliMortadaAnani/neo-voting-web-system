using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Contracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;
using NeoVoting.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class CandidateServices : ICandidateServices
    {   
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ISystemAuditLogRepository _systemAuditLogRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IGovernmentSystemGateway _governmentSystemGateway;
        private readonly IElectionRepository _electionRepository;

        public CandidateServices(IUnitOfWork unitOfWork,
                                 ICandidateProfileRepository candidateProfileRepository,
                                 ISystemAuditLogRepository systemAuditLogRepository,
                                 ICurrentUserServices currentUserServices,
                                 IGovernmentSystemGateway governmentSystemGateway,
                                 IElectionRepository electionRepository)
        {
            _unitOfWork = unitOfWork;
            _candidateProfileRepository = candidateProfileRepository;
            _systemAuditLogRepository = systemAuditLogRepository;
            _currentUserServices = currentUserServices;
            _governmentSystemGateway = governmentSystemGateway;
            _electionRepository = electionRepository;
        }

        public async Task<Result<CandidateProfile_ResponseDTO>> AddCandidateProfileByElectionIdAsync(Guid electionId, CandidateProfileAdd_RequestDTO request, CancellationToken cancellationToken)
        {
            // In your service or controller
            var election = await _electionRepository.GetElectionByIdAsync(electionId,cancellationToken);
            if(election == null)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.NotFound
                    (
                    nameof(ProblemDetails404ErrorTypes.Election_NotFound),
                    "Election not found."));

            }
            
            
            if(election.ElectionStatusId != (int)ElectionStatusEnum.Nomination)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Forbidden
                    (
                    nameof(ProblemDetails403ErrorTypes.CandidateProfile_NotInNominationPhase),
                    "Candidate profiles can only be created during the Nomination phase of the election."));
            }

            var currentUserId = _currentUserServices.GetAuthenticatedUserId();
            var currentUsername = _currentUserServices.GetAuthenticatedUsername();
            var isProfileExist = await _candidateProfileRepository.IsCandidateProfileExistsByUserIdAndElectionIdAsync(electionId, currentUserId, cancellationToken);
            if (isProfileExist)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Conflict
                    (
                    nameof(ProblemDetails409ErrorTypes.CandidateProfile_AlreadyExisted),
                    "Candidate profile already exists for this election."));
            }

            var verifyRequest = new NeoVoting_GetCandidateRequestDTO
            {
                NationalId = request.NationalId,
                NominationToken = request.NominationToken
            };

            // CALL GATEWAY: This handles the HTTP Post, Try/Catch, and JSON Parsing.
            var verifyResult = await _governmentSystemGateway.GetCandidateAsync(verifyRequest, cancellationToken);

            // If the network failed, or the API returned 404/400/500, we stop here.
            if (verifyResult.IsFailure)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(verifyResult.Error);
            }

            var govData = verifyResult.Value; // This contains Name, DOB, Eligibility, etc.

            if (govData.RegisteredUsername != currentUsername)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Unauthorized
                    (
                    nameof(ProblemDetails401ErrorTypes.Auth_UnauthorizedAccess),
                    "The provided National ID or Nomination Token does not match the authenticated user."
                    ));

            }
            var candidateProfile = CandidateProfile.Create(
                currentUserId,
                electionId,
                request.Goals!,
                request.NominationReasons!
                );

            var newProfile = await _candidateProfileRepository.AddCandidateProfileAsync(candidateProfile , cancellationToken);

            if(newProfile == null)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Failure
                    (
                    nameof(ProblemDetails500ErrorTypes.CandidateProfile_CreationFailed),
                    "Failed to create candidate profile due to an internal error."
                    ));
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var responseDto = new CandidateProfile_ResponseDTO
            {
                Id = newProfile.Id,
                UserId = newProfile.UserId,
                ElectionId = newProfile.ElectionId,
                Goals = newProfile.Goals,
                NominationReasons = newProfile.NominationReasons,
                ProfilePhotoFilename = newProfile.ProfilePhotoFilename
            };
            return Result<CandidateProfile_ResponseDTO>.Success(responseDto);

        } 

        public Task<Result<CandidateProfile_ResponseDTO>> GetCandidateProfileByElectionIdAsync(Guid electionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> UpdateCandidateProfileByElectionIdAsync(Guid electionId, CandidateProfileUpdate_RequestDTO request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> UpdateCandidateProfile_Photo_ByElectionIdAsync(Guid electionId, CandidateProfileUploadImage_RequestDTO request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
