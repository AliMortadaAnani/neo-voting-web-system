using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.CandidateDTOs;
using GovernmentSystem.API.Domain.ResultErrorDomain;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface ICandidateServices
    {
        Task<Result<CandidateResponseDTO>> GetCandidateByNationalIdAsync(GetCandidateRequestDTO request);

        Task<Result<CandidateResponseDTO>> GetCandidateByHashedDataAsync(GetCandidateRequestDTO request);

        Task<Result<PagedResult<CandidateResponseDTO>>> GetCandidatesPagedAsync(int pageNumber, int pageSize);

      

        Task<Result<CandidateResponseDTO>> AddCandidateAsync(CreateCandidateRequestDTO request);

        Task<Result<bool>> DeleteCandidateByNationalIdAsync(DeleteCandidateRequestDTO request);

        Task<Result<CandidateResponseDTO>> GenerateNewNominationTokenByNationalIdAsync(UpdateCandidateRequestDTO request);

        Task <Result<CandidateVerifyResponseDTO>> VerifyCandidateCredentialsAsync(GetCandidateVerificationRequestDTO request);

    }
}