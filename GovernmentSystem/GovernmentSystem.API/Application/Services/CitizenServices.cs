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

        public CitizenServices(ICitizenRepository citizenRepository, IUnitOfWork unitOfWork, SensitiveDataHelper sensitiveDataHelper)
        {
            _citizenRepository = citizenRepository;
            _unitOfWork = unitOfWork;
            _sensitiveDataHelper = sensitiveDataHelper;
        }

        public async Task<Result<CitizenResponseDTO>> GetCitizenByNationalIdAsync(GetCitizenRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                return Result<CitizenResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            var response = citizen.ToCitizenResponse(_sensitiveDataHelper);
            return Result<CitizenResponseDTO>.Success(response);
        }

        public async Task<Result<PagedResult<CitizenResponseDTO>>> GetCitizensPagedAsync(int pageNumber, int pageSize)
        {   // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                return Result<PagedResult<CitizenResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                return Result<PagedResult<CitizenResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageSize must be greater than 0."));
            }

            // 2. SECURITY: Cap the PageSize
            // If they ask for 5000, force it down to 100 to protect RAM/Network.
            if (pageSize > 100) pageSize = 100;

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

            return Result<PagedResult<CitizenResponseDTO>>.Success(pagedResult);
        }
        public async Task<Result<CitizenResponseDTO>> AddCitizenAsync(CreateCitizenRequestDTO request)
        {
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

            var response = citizen.ToCitizenResponse(_sensitiveDataHelper);

            return Result<CitizenResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteCitizenByNationalIdAsync(DeleteCitizenRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                return Result<bool>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            _citizenRepository.Delete(citizen);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<CitizenResponseDTO>> UpdateCitizenByNationalIdAsync(UpdateCitizenRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
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

            var response = citizen.ToCitizenResponse(_sensitiveDataHelper);
            return Result<CitizenResponseDTO>.Success(response);
        }


        public async Task<Result<int>> GetCitizensTotalCountAsync()
        {
            var totalCount = await _citizenRepository.CountAsync();
            return Result<int>.Success(totalCount);
        }

    }
}