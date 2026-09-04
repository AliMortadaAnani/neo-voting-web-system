using NeoVoting.Application.RequestDTOs.CandidateDTOs;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.CandidateDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly ICandidateProfileRepository _candidateProfileRepository;

    public CandidateServices(ICandidateRepository candidateRepository, IElectionRepository electionRepository, IUnitOfWork unitOfWork, ICandidateProfileRepository candidateProfileRepository, ICurrentUserServices currentUserServices)
    {
        _candidateRepository = candidateRepository;
        _electionRepository = electionRepository;
        _unitOfWork = unitOfWork;
        _currentUserServices = currentUserServices;
        _candidateProfileRepository = candidateProfileRepository;
         

    }
    
        public async Task<Result<CandidateProfile_ResponseDTO>> CreateCandidateProfileAsync(int electionId, CandidateProfile_Create_Update_RequestDTO dto)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.NotFound(
                    nameof(ProblemDetails404ErrorTypes.Election_NotFound),
                    "Election not found."
                ));
            }

            if (election.Status == StatusEnum.Completed)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Nominate_CompletedElection),
                    "Election is already completed."
                ));
            }
            if (election.Status == StatusEnum.Voting)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Nominate_StartedElection),
                    "Election is already in the voting phase."
                ));
            }
            if (election.Status != StatusEnum.Upcoming)
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Failure(
                    nameof(ProblemDetails500ErrorTypes.Server_Error),
                    "Election is in an unknown phase."
                ));
            }

            int candidateId = (int)_currentUserServices.AccountId!;

            if (await _candidateProfileRepository.IsCandidateProfileExistsByCandidateIdAndElectionIdAsync(electionId, candidateId))
            {
                return Result<CandidateProfile_ResponseDTO>.Failure(Error.Conflict(
                    nameof(ProblemDetails409ErrorTypes.CandidateProfile_AlreadyExisted),
                    "Candidate profile already exists for this election."
                ));
            }



            var candidateProfile = CandidateProfile.Create(candidateId, electionId, dto.Goals!, dto.NominationReasons!);

            _candidateProfileRepository.Add(candidateProfile);
    
            await _unitOfWork.SaveChangesAsync();

            var addedCandidateProfile = await _candidateProfileRepository.GetByCandidateIdAndElectionIdAsync(electionId, candidateId);

            var responseDTO = new CandidateProfile_ResponseDTO
            {
                CandidateProfileId = addedCandidateProfile!.Id,
                Goals = addedCandidateProfile.Goals,
                NominationReasons = addedCandidateProfile.NominationReasons,
                ElectionId = addedCandidateProfile.ElectionId,
                ElectionName = election.Name,
                Governorate = addedCandidateProfile.Candidate.Governorate,
                CandidateId = addedCandidateProfile.CandidateId,
                ApplicationUserId = addedCandidateProfile.Candidate.UserId,
                Username = addedCandidateProfile.Candidate.User.UserName,
                FirstName = addedCandidateProfile.Candidate.FirstName,
                LastName = addedCandidateProfile.Candidate.LastName,
                Gender = addedCandidateProfile.Candidate.Gender
            };
            return Result<CandidateProfile_ResponseDTO>.Success(responseDTO);
        }

        public Task<Result<CandidateProfile_ResponseDTO>> GetCandidateProfileAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> RemoveImageForCandidateProfileAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> UpdateCandidateProfileAsync(int electionId, CandidateProfile_Create_Update_RequestDTO candidateRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateProfile_ResponseDTO>> UpdateImageForCandidateProfileAsync(int electionId, CandidateProfileUploadImage_RequestDTO candidateRequestDTO)
        {
            throw new NotImplementedException();
        }
    }

}