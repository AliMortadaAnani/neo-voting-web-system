using System.Collections.Generic;
using System.Threading.Tasks;
using GovernmentSystem.API.Domain.Shared;
using GovernmentSystem.Application.RequestDTOs;
using GovernmentSystem.Application.ResponseDTOs;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface IVoterServices
    {
        Task<Result<VoterResponseDTO>> GetByNationalIdAsync(GetVoterRequestDTO request);
        Task<Result<List<VoterResponseDTO>>> GetAllVotersAsync();
        Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request);
        Task<Result<VoterResponseDTO>> UpdateByNationalIdAsync(UpdateVoterRequestDTO request);
        Task<Result<bool>> DeleteByNationalIdAsync(DeleteVoterRequestDTO request);
        Task<Result<VoterResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenVoterRequestDTO request);

        Task<Result<bool>> UpdateVoterIsVotedFieldAsync(NeoVoting_VoterHasVotedRequestDTO request);
        Task<Result<bool>> UpdateVoterIsRegisteredFieldAsync(NeoVoting_VoterIsRegisteredRequestDTO request);
        Task<Result<NeoVoting_VoterResponseDTO>> GetVoterForNeoVotingAsync(NeoVoting_GetVoterRequestDTO request);
        Task<Result<bool>> ResetAllVotedAsFalseAsync();
    }
}
