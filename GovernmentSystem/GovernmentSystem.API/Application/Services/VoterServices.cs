using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Shared;
using GovernmentSystem.Application.RequestDTOs;
using GovernmentSystem.Application.ResponseDTOs;

namespace GovernmentSystem.API.Application.Services
{
    public class VoterServices : IVoterServices
    {
        public VoterServices()
        {
        }

        public Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteByNationalIdAsync(DeleteVoterRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<VoterResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenVoterRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<VoterResponseDTO>>> GetAllVotersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<VoterResponseDTO>> GetByNationalIdAsync(GetVoterRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<NeoVoting_VoterResponseDTO>> GetVoterForNeoVotingAsync(NeoVoting_GetVoterRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> ResetAllVotedAsFalseAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<VoterResponseDTO>> UpdateByNationalIdAsync(UpdateVoterRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateVoterIsRegisteredFieldAsync(NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateVoterIsVotedFieldAsync(NeoVoting_VoterHasVotedRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
