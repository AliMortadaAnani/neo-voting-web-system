using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.ServicesContracts
{
    public interface INeoVotingServices
    {
        Task<Result<NeoVoting_VoterResponseDTO>> UpdateHasVotedToTrueAsync(NeoVoting_VoterHasVotedRequestDTO request);

        Task<Result<NeoVoting_VoterResponseDTO>> UpdateVoterIsRegisteredToTrueAsync(NeoVoting_VoterIsRegisteredRequestDTO request);

        Task<Result<NeoVoting_VoterResponseDTO>> GetVoterForNeoVotingAsync(NeoVoting_GetVoterRequestDTO request);

        Task<Result<bool>> ResetAllVotedAsFalseAsync();

        Task<Result<NeoVoting_CandidateResponseDTO>> UpdateCandidateIsRegisteredToTrueAsync(NeoVoting_CandidateIsRegisteredRequestDTO request);

        Task<Result<NeoVoting_CandidateResponseDTO>> GetCandidateForNeoVotingAsync(NeoVoting_GetCandidateRequestDTO request);
    }
}
