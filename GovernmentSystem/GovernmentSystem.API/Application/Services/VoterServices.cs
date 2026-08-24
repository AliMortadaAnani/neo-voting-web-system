using GovernmentSystem.API.Application.Helpers;
using GovernmentSystem.API.Application.RequestDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.VoterDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.Services
{
    public class VoterServices : IVoterServices
    {
        private readonly IVoterRepository _voterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SensitiveDataHelper _sensitiveDataHelper;
        private readonly ICitizenRepository _citizenRepository;
        private readonly ILogger<VoterServices> _logger;

        public VoterServices(IVoterRepository voterRepository, ICitizenRepository citizenRepository, IUnitOfWork unitOfWork, SensitiveDataHelper sensitiveDataHelper, ILogger<VoterServices> logger)
        {
            _voterRepository = voterRepository;
            _unitOfWork = unitOfWork;
            _sensitiveDataHelper = sensitiveDataHelper;
            _citizenRepository = citizenRepository;
            _logger = logger;
        }

        public async Task<Result<VoterResponseDTO>> GetVoterByNationalIdAsync(GetVoterRequestDTO request)
        {
            _logger.LogInformation("GetVoterByNationalId operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var voter = await _voterRepository.GetVoterByNationalIdAsync(encryptedNationalId);

            if (voter == null)
            {
                _logger.LogWarning("Voter not found for GetVoterByNationalId operation");
                return Result<VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter not found."));
            }

            _logger.LogInformation("Voter retrieved successfully for GetVoterByNationalId operation");
            var response = voter.ToVoterResponse(_sensitiveDataHelper);

            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<PagedResult<VoterResponseDTO>>> GetVotersPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("GetVotersPaged operation initiated - PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                _logger.LogWarning("GetVotersPaged validation failed - invalid PageNumber: {PageNumber}", pageNumber);
                return Result<PagedResult<VoterResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                _logger.LogWarning("GetVotersPaged validation failed - invalid PageSize: {PageSize}", pageSize);
                return Result<PagedResult<VoterResponseDTO>>.Failure(
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
            int totalCount = await _voterRepository.CountAsync();

            var voters = await _voterRepository.GetPagedAsync(pageNumber, pageSize);
            var response = voters.Select(v => v.ToVoterResponse(_sensitiveDataHelper)).ToList();

            var pagedResult = new PagedResult<VoterResponseDTO>
            {
                Data = response,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            _logger.LogInformation("GetVotersPaged operation successful - Retrieved {Count} records out of {TotalCount} total", response.Count, totalCount);
            return Result<PagedResult<VoterResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<VoterVerifyResponseDTO>> VerifyVoterCredentialsAsync(GetVoterVerificationRequestDTO request)
        {
            _logger.LogInformation("VerifyVoterCredentials operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);
            string encryptedVotingToken = _sensitiveDataHelper.Encrypt(request.VotingToken!);
            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedVotingToken);

            var voter = await _voterRepository.GetVoterByHashedDataAsync(hashedData);

            if (voter == null)
            {
                _logger.LogWarning("Voter verification failed - invalid credentials");
                return Result<VoterVerifyResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_InvalidCredentials), "Voter invalid credentials."));
            }

            _logger.LogInformation("Voter verification successful");
            var response = voter.ToNeoVoting_VoterResponse();
            return Result<VoterVerifyResponseDTO>.Success(response);
        }

        public async Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request)
        {
            _logger.LogInformation("AddVoter operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                _logger.LogWarning("AddVoter failed - citizen not found");
                return Result<VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            bool isVoterExists = await _voterRepository.IsVoterExistByNationalIdAsync(encryptedNationalId);

            if (isVoterExists)
            {
                _logger.LogWarning("AddVoter failed - voter already registered");
                return Result<VoterResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyRegistered), "Voter already registered."));
            }

            string rawVotingToken = _sensitiveDataHelper.GenerateVotingToken
               (citizen.FirstName,
               citizen.LastName,
               (int)citizen.Governorate,
               citizen.Gender,
               citizen.DateOfBirth);

            string encryptedVotingToken = _sensitiveDataHelper.Encrypt(rawVotingToken);

            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedVotingToken);
            var voter = Voter.Create(
                encryptedVotingToken,
                hashedData,
                citizen.Id
                 );

            _voterRepository.Add(voter);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("AddVoter operation successful");
            var response = voter.ToVoterResponse(_sensitiveDataHelper);

            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteVoterByNationalIdAsync(DeleteVoterRequestDTO request)
        {
            _logger.LogInformation("DeleteVoter operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var voter = await _voterRepository.GetVoterByNationalIdAsync(encryptedNationalId);

            if (voter == null)
            {
                _logger.LogWarning("DeleteVoter failed - voter not found");
                return Result<bool>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter not found."));
            }

            _voterRepository.Delete(voter);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("DeleteVoter operation successful");
            return Result<bool>.Success(true);
        }

        public async Task<Result<VoterResponseDTO>> GenerateNewVotingTokenByNationalIdAsync(UpdateVoterRequestDTO request)
        {
            _logger.LogInformation("GenerateNewVotingToken operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var voter = await _voterRepository.GetVoterByNationalIdAsync(encryptedNationalId);

            if (voter == null)
            {
                _logger.LogWarning("GenerateNewVotingToken failed - voter not found");
                return Result<VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter not found."));
            }
            string rawVotingToken = _sensitiveDataHelper.GenerateVotingToken
              (voter.Citizen.FirstName,
              voter.Citizen.LastName,
              (int)voter.Citizen.Governorate,
              voter.Citizen.Gender,
              voter.Citizen.DateOfBirth);

            string encryptedVotingToken = _sensitiveDataHelper.Encrypt(rawVotingToken);

            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedVotingToken);

            voter.Update(encryptedVotingToken, hashedData);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("GenerateNewVotingToken operation successful");
            var response = voter.ToVoterResponse(_sensitiveDataHelper);

            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<int>> GetVotersTotalCountAsync()
        {
            _logger.LogInformation("GetVotersTotalCount operation initiated");
            int totalCount = await _voterRepository.CountAsync();
            _logger.LogInformation("GetVotersTotalCount operation successful - Total: {TotalCount}", totalCount);
            return Result<int>.Success(totalCount);
        }
    }
}