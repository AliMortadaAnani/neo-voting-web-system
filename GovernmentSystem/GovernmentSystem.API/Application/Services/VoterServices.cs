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

        public VoterServices(IVoterRepository voterRepository, ICitizenRepository citizenRepository, IUnitOfWork unitOfWork, SensitiveDataHelper sensitiveDataHelper)
        {
            _voterRepository = voterRepository;
            _unitOfWork = unitOfWork;
            _sensitiveDataHelper = sensitiveDataHelper;
            _citizenRepository = citizenRepository;
        }

        public async Task<Result<VoterResponseDTO>> GetVoterByNationalIdAsync(GetVoterRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var voter = await _voterRepository.GetVoterByNationalIdAsync(encryptedNationalId);

            if (voter == null)
            {
                return Result<VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter not found."));
            }

            var response = voter.ToVoterResponse(_sensitiveDataHelper);

            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<PagedResult<VoterResponseDTO>>> GetVotersPagedAsync(int pageNumber, int pageSize)
        {
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                return Result<PagedResult<VoterResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                return Result<PagedResult<VoterResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageSize must be greater than 0."));
            }

            // 2. SECURITY: Cap the PageSize
            // If they ask for 5000, force it down to 100 to protect RAM/Network.
            if (pageSize > 100) pageSize = 100;

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

            return Result<PagedResult<VoterResponseDTO>>.Success(pagedResult);
        }

        public async Task<Result<VoterVerifyResponseDTO>> VerifyVoterCredentialsAsync(GetVoterVerificationRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);
            string encryptedVotingToken = _sensitiveDataHelper.Encrypt(request.VotingToken!);
            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedVotingToken);

            var voter = await _voterRepository.GetVoterByHashedDataAsync(hashedData);

            if (voter == null)
            {
                return Result<VoterVerifyResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails401ErrorTypes.Voter_InvalidCredentials), "Voter invalid credentials."));
            }

            var response = voter.ToNeoVoting_VoterResponse();
            return Result<VoterVerifyResponseDTO>.Success(response);
        }

        public async Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var citizen = await _citizenRepository.GetCitizenByNationalIdAsync(encryptedNationalId);

            if (citizen == null)
            {
                return Result<VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Citizen_NotFound), "Citizen not found."));
            }

            bool isVoterExists = await _voterRepository.IsVoterExistByNationalIdAsync(encryptedNationalId);

            if (isVoterExists)
            {
                return Result<VoterResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyRegistered), "Voter already registered."));
            }

            string rawVotingToken = _sensitiveDataHelper.GenerateVotingToken
               (citizen.FirstName,
               citizen.LastName,
               (int)citizen.GovernorateId,
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

            var response = voter.ToVoterResponse(_sensitiveDataHelper);

            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteVoterByNationalIdAsync(DeleteVoterRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var voter = await _voterRepository.GetVoterByNationalIdAsync(encryptedNationalId);

            if (voter == null)
            {
                return Result<bool>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter not found."));
            }

            _voterRepository.Delete(voter);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<VoterResponseDTO>> GenerateNewVotingTokenByNationalIdAsync(UpdateVoterRequestDTO request)
        {
            string encryptedNationalId = _sensitiveDataHelper.Encrypt(request.NationalId!);

            var voter = await _voterRepository.GetVoterByNationalIdAsync(encryptedNationalId);

            if (voter == null)
            {
                return Result<VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter not found."));
            }
            string rawVotingToken = _sensitiveDataHelper.GenerateVotingToken
              (voter.Citizen.FirstName,
              voter.Citizen.LastName,
              (int)voter.Citizen.GovernorateId,
              voter.Citizen.Gender,
              voter.Citizen.DateOfBirth);

            string encryptedVotingToken = _sensitiveDataHelper.Encrypt(rawVotingToken);

            string hashedData = _sensitiveDataHelper.HashData(encryptedNationalId, encryptedVotingToken);

            voter.Update(encryptedVotingToken, hashedData);

            await _unitOfWork.SaveChangesAsync();

            var response = voter.ToVoterResponse(_sensitiveDataHelper);

            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<int>> GetVotersTotalCountAsync()
        {
            int totalCount = await _voterRepository.CountAsync();
            return Result<int>.Success(totalCount);
        }
    }
}