using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        public CandidateServices()
        {
        }

        public Task<Result<CandidateResponseDTO>> AddCandidateAsync(CreateCandidateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteByNationalIdAsync(DeleteCandidateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenCandidateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CandidateResponseDTO>>> GetAllCandidatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateResponseDTO>> GetByNationalIdAsync(GetCandidateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<NeoVoting_CandidateResponseDTO>> GetCandidateForNeoVotingAsync(NeoVoting_GetCandidateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CandidateResponseDTO>> UpdateByNationalIdAsync(UpdateCandidateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<NeoVoting_CandidateResponseDTO>> UpdateCandidateIsRegisteredFieldAsync(NeoVoting_CandidateIsRegisteredRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
