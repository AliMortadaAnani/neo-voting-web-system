using GovernmentSystem.API.Application.Helpers;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SensitiveDataHelper _sensitiveDataHelper;
        private readonly ICitizenRepository _citizenRepository;
        private readonly ILogger<CandidateServices> _logger;

        public CandidateServices(ICandidateRepository candidateRepository, ICitizenRepository citizenRepository, IUnitOfWork unitOfWork, SensitiveDataHelper sensitiveDataHelper, ILogger<CandidateServices> logger)
        {
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
            _sensitiveDataHelper = sensitiveDataHelper;
            _citizenRepository = citizenRepository;
            _logger = logger;
        }

        public async Task<Result<CandidateResponseDTO>> GetCandidateByNationalIdAsync(GetCandidateRequestDTO request)
        {
            _logger.LogInformation("GetCandidateByNationalId operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(encryptedNationalId);

            if (candidate == null)
            {
                _logger.LogWarning("Candidate not found for GetCandidateByNationalId operation");
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }

            _logger.LogInformation("Candidate retrieved successfully for GetCandidateByNationalId operation");
            var response = candidate.ToCandidateResponse(_sensitiveDataHelper);

            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<PagedResult<CandidateResponseDTO>>> GetCandidatesPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("GetCandidatesPaged operation initiated - PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                _logger.LogWarning("GetCandidatesPaged validation failed - invalid PageNumber: {PageNumber}", pageNumber);
                return Result<PagedResult<CandidateResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                _logger.LogWarning("GetCandidatesPaged validation failed - invalid PageSize: {PageSize}", pageSize);
                return Result<PagedResult<CandidateResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageSize must be greater than 0."));
            }

            // 2. SECURITY: Cap the PageSize
            // If they ask for 5000, force it down to 100 to protect RAM/Network.
            if (pageSize > 100)
            {
                pageSize = 100;
                _logger.LogInformation("PageSize capped to 100");
            }

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

            _logger.LogInformation("GetCandidatesPaged operation successful - Retrieved {Count} records out of {TotalCount} total", response.Count, totalCount);
            return Result<PagedResult<CandidateResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<CandidateVerifyResponseDTO>> VerifyCandidateCredentialsAsync(GetCandidateVerificationRequestDTO request)
        {
            _logger.LogInformation("VerifyCandidateCredentials operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);
            string encryptedNominationToken = _sensitiveDataHelper.Encrypt(request.NominationToken!);
            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedNominationToken);

            var candidate = await _candidateRepository.GetCandidateByHashedDataAsync(hashedData);

            if (candidate == null)
            {
                _logger.LogWarning("Candidate verification failed - invalid credentials");
                return Result<CandidateVerifyResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails401ErrorTypes.Candidate_InvalidCredentials), "Candidate invalid credentials."));
            }

            _logger.LogInformation("Candidate verification successful");
            var response = candidate.ToNeoVoting_CandidateResponse();
            return Result<CandidateVerifyResponseDTO>.Success(response);
        }

        public async Task<Result<CandidateResponseDTO>> AddCandidateAsync(CreateCandidateRequestDTO request)
        {
            _logger.LogInformation("AddCandidate operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                _logger.LogWarning("AddCandidate failed - citizen not found");
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            bool isCandidateExists = await _candidateRepository.IsCandidateExistByNationalIdAsync(encryptedNationalId);

            if (isCandidateExists)
            {
                _logger.LogWarning("AddCandidate failed - candidate already registered");
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

            _logger.LogInformation("AddCandidate operation successful");
            var response = candidate.ToCandidateResponse(_sensitiveDataHelper);

            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteCandidateByNationalIdAsync(DeleteCandidateRequestDTO request)
        {
            _logger.LogInformation("DeleteCandidate operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(encryptedNationalId);

            if (candidate == null)
            {
                _logger.LogWarning("DeleteCandidate failed - candidate not found");
                return Result<bool>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }

            _candidateRepository.Delete(candidate);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("DeleteCandidate operation successful");
            return Result<bool>.Success(true);
        }

        public async Task<Result<CandidateResponseDTO>> GenerateNewNominationTokenByNationalIdAsync(UpdateCandidateRequestDTO request)
        {
            _logger.LogInformation("GenerateNewNominationToken operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(encryptedNationalId);

            if (candidate == null)
            {
                _logger.LogWarning("GenerateNewNominationToken failed - candidate not found");
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

            _logger.LogInformation("GenerateNewNominationToken operation successful");
            var response = candidate.ToCandidateResponse(_sensitiveDataHelper);

            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<int>> GetCandidatesTotalCountAsync()
        {
            _logger.LogInformation("GetCandidatesTotalCount operation initiated");
            int totalCount = await _candidateRepository.CountAsync();
            _logger.LogInformation("GetCandidatesTotalCount operation successful - Total: {TotalCount}", totalCount);
            return Result<int>.Success(totalCount);
        }
    }
}