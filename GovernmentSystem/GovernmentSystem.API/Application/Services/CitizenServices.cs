using GovernmentSystem.API.Application.Helpers;
using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CitizenDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.Services
{
    public class CitizenServices : ICitizenServices
    {
        private readonly ICitizenRepository _citizenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SensitiveDataHelper _sensitiveDataHelper;
        private readonly ILogger<CitizenServices> _logger;

        public CitizenServices(ICitizenRepository citizenRepository, IUnitOfWork unitOfWork, SensitiveDataHelper sensitiveDataHelper, ILogger<CitizenServices> logger)
        {
            _citizenRepository = citizenRepository;
            _unitOfWork = unitOfWork;
            _sensitiveDataHelper = sensitiveDataHelper;
            _logger = logger;
        }

        public async Task<Result<CitizenResponseDTO>> GetCitizenByNationalIdAsync(GetCitizenRequestDTO request)
        {
            _logger.LogInformation("GetCitizenByNationalId operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                _logger.LogWarning("Citizen not found for GetCitizenByNationalId operation");
                return Result<CitizenResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            _logger.LogInformation("Citizen retrieved successfully for GetCitizenByNationalId operation");
            var response = citizen.ToCitizenResponse(_sensitiveDataHelper);
            return Result<CitizenResponseDTO>.Success(response);
        }

        public async Task<Result<PagedResult<CitizenResponseDTO>>> GetCitizensPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("GetCitizensPaged operation initiated - PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                _logger.LogWarning("GetCitizensPaged validation failed - invalid PageNumber: {PageNumber}", pageNumber);
                return Result<PagedResult<CitizenResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                _logger.LogWarning("GetCitizensPaged validation failed - invalid PageSize: {PageSize}", pageSize);
                return Result<PagedResult<CitizenResponseDTO>>.Failure(
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
            int totalCount = await _citizenRepository.CountAsync();

            var citizens = await _citizenRepository.GetPagedAsync(pageNumber, pageSize);
            var response = citizens.Select(c => c.ToCitizenResponse(_sensitiveDataHelper)).ToList();

            var pagedResult = new PagedResult<CitizenResponseDTO>
            {
                Data = response,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            _logger.LogInformation("GetCitizensPaged operation successful - Retrieved {Count} records out of {TotalCount} total", response.Count, totalCount);
            return Result<PagedResult<CitizenResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<CitizenResponseDTO>> AddCitizenAsync(CreateCitizenRequestDTO request)
        {
            _logger.LogInformation("AddCitizen operation initiated");
            string rawNationalId = _sensitiveDataHelper.GenerateNationalId
                (request.FirstName!,
                request.LastName!,
                (int)request.GovernorateId!.Value,
                request.Gender!.Value,
                request.DateOfBirth!.Value);

            string encryptedNationalId = _sensitiveDataHelper.Encrypt(rawNationalId);

            var citizen = Citizen.Create(
                encryptedNationalId,
                request.FirstName!,
                request.LastName!,
                request.DateOfBirth!.Value,
                request.Gender!.Value,
                request.GovernorateId!.Value
                 );

            _citizenRepository.Add(citizen);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("AddCitizen operation successful");
            var response = citizen.ToCitizenResponse(_sensitiveDataHelper);

            return Result<CitizenResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteCitizenByNationalIdAsync(DeleteCitizenRequestDTO request)
        {
            _logger.LogInformation("DeleteCitizen operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                _logger.LogWarning("DeleteCitizen failed - citizen not found");
                return Result<bool>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            _citizenRepository.Delete(citizen);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("DeleteCitizen operation successful");
            return Result<bool>.Success(true);
        }

        public async Task<Result<CitizenResponseDTO>> UpdateCitizenByNationalIdAsync(UpdateCitizenRequestDTO request)
        {
            _logger.LogInformation("UpdateCitizen operation initiated");
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                _logger.LogWarning("UpdateCitizen failed - citizen not found");
                return Result<CitizenResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            // Update the citizen's properties based on the request
            citizen.Update(
                request.FirstName!,
                request.LastName!,
                request.DateOfBirth!.Value,
                request.Gender!.Value,
                request.GovernorateId!.Value
            );

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("UpdateCitizen operation successful");
            var response = citizen.ToCitizenResponse(_sensitiveDataHelper);
            return Result<CitizenResponseDTO>.Success(response);
        }

        public async Task<Result<int>> GetCitizensTotalCountAsync()
        {
            _logger.LogInformation("GetCitizensTotalCount operation initiated");
            var totalCount = await _citizenRepository.CountAsync();
            _logger.LogInformation("GetCitizensTotalCount operation successful - Total: {TotalCount}", totalCount);
            return Result<int>.Success(totalCount);
        }
    }
}