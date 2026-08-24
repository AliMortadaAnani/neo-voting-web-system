using GovernmentSystem.API.Application.Helpers;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.Enums;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using GovernmentSystem.API.Infrastructure.Repositories;

namespace GovernmentSystem.API.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SensitiveDataHelper _sensitiveDataHelper;
        private readonly ICitizenRepository _citizenRepository;


        public CandidateServices(ICandidateRepository candidateRepository, ICitizenRepository citizenRepository, IUnitOfWork unitOfWork, SensitiveDataHelper sensitiveDataHelper)
        {
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
            _sensitiveDataHelper = sensitiveDataHelper;
            _citizenRepository = citizenRepository;
        }


        public async Task<Result<CandidateResponseDTO>> GetCandidateByNationalIdAsync(GetCandidateRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(encryptedNationalId);

            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }

            var response = candidate.ToCandidateResponse(_sensitiveDataHelper);

            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<PagedResult<CandidateResponseDTO>>> GetCandidatesPagedAsync(int pageNumber, int pageSize)
        {
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                return Result<PagedResult<CandidateResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                return Result<PagedResult<CandidateResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageSize must be greater than 0."));
            }

            // 2. SECURITY: Cap the PageSize
            // If they ask for 5000, force it down to 100 to protect RAM/Network.
            if (pageSize > 100) pageSize = 100;

            // 3. Get total count
            int totalCount = await _candidateRepository.CountAsync();

            var candidates = await _candidateRepository.GetPagedAsync(pageNumber, pageSize);
            var response = candidates.Select(c => c.ToCandidateResponse(_sensitiveDataHelper)).ToList();

            var pagedResult = new PagedResult<CandidateResponseDTO>
            {
                Data = response,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<CandidateResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<CandidateVerifyResponseDTO>> VerifyCandidateCredentialsAsync(GetCandidateVerificationRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);
            string encryptedNominationToken = _sensitiveDataHelper.Encrypt(request.NominationToken!);
            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedNominationToken);

            var candidate = await _candidateRepository.GetCandidateByHashedDataAsync(hashedData);

            if (candidate == null)
            {
                return Result<CandidateVerifyResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }

            var response = candidate.ToNeoVoting_CandidateResponse();
            return Result<CandidateVerifyResponseDTO>.Success(response);
        }
        public async Task<Result<CandidateResponseDTO>> AddCandidateAsync(CreateCandidateRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            bool isCandidateExists = await _candidateRepository.IsCandidateExistByNationalIdAsync(encryptedNationalId);

            if (isCandidateExists)
            {
                return Result<CandidateResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Candidate_AlreadyRegistered), "Candidate already registered."));
            }

            string rawNominationToken = _sensitiveDataHelper.GenerateNominationToken
               (citizen.FirstName,
               citizen.LastName,
               (int)citizen.GovernorateId,
               citizen.Gender,
               citizen.DateOfBirth);

            string encryptedNominationToken = _sensitiveDataHelper.Encrypt(rawNominationToken);

            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedNominationToken);

            var candidate = Candidate.Create(
                encryptedNominationToken,
                hashedData,
                citizen.Id
                 );

            _candidateRepository.Add(candidate);

            await _unitOfWork.SaveChangesAsync();

            var response = candidate.ToCandidateResponse(_sensitiveDataHelper);

            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteCandidateByNationalIdAsync(DeleteCandidateRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(encryptedNationalId);

            if (candidate == null)
            {
                return Result<bool>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }

            _candidateRepository.Delete(candidate);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<CandidateResponseDTO>> GenerateNewNominationTokenByNationalIdAsync(UpdateCandidateRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(encryptedNationalId);

            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }
            string rawNominationToken = _sensitiveDataHelper.GenerateNominationToken
              (candidate.Citizen.FirstName,
              candidate.Citizen.LastName,
              (int)candidate.Citizen.GovernorateId,
              candidate.Citizen.Gender,
              candidate.Citizen.DateOfBirth);

            string encryptedNominationToken = _sensitiveDataHelper.Encrypt(rawNominationToken);

            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedNominationToken);

            candidate.Update(encryptedNominationToken, hashedData);

            await _unitOfWork.SaveChangesAsync();

            var response = candidate.ToCandidateResponse(_sensitiveDataHelper);

            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<int>> GetCandidatesTotalCountAsync()
        {
            int totalCount = await _candidateRepository.CountAsync();
            return Result<int>.Success(totalCount);
        }

    }
}