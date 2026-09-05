using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class CandidateServices : ICandidateServices
    {   
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly ICurrentUserServices _currentUserServices;

        public CandidateServices(IUnitOfWork unitOfWork, ICandidateProfileRepository candidateProfileRepository, ICurrentUserServices currentUserServices, IElectionRepository electionRepository)
        {
            _unitOfWork = unitOfWork;
            _candidateProfileRepository = candidateProfileRepository;
            _currentUserServices = currentUserServices;
            _electionRepository = electionRepository;
        }
        public async Task<Result<CandidateProfile_ResponseDTO>> CreateCandidateProfileAsync(int electionId, CandidateProfile_Create_RequestDTO candidateRequestDTO)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));
            }

            if(election.Status != StatusEnum.Upcoming)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.ElectionInvalidState), "Election is not in upcoming phase."));
            }

            int currentUserId = (int)_currentUserServices.AccountId!;

            if(await _candidateProfileRepository.IsCandidateProfileExistsByCandidateIdAndElectionIdAsync(currentUserId, electionId))
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.CandidateProfile_AlreadyExisted), "Candidate profile already exists for this user in the election."));
            }

            var profile = CandidateProfile.Create(currentUserId, electionId, candidateRequestDTO.NominationReasons!);

            _candidateProfileRepository.Add(profile);

            await _unitOfWork.SaveChangesAsync();

            var addedProfile = await _candidateProfileRepository.GetByCandidateIdAndElectionIdAsync(currentUserId, electionId);

            if(addedProfile == null)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Failed to create candidate profile."));
            }

            var responseDTO = addedProfile!.ToCandidateProfileResponse(null);

            return Result<CandidateProfile_ResponseDTO>.Success(responseDTO);

        }
    }
}