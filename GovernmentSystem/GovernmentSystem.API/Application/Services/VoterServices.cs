using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Contracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.Shared;


namespace GovernmentSystem.API.Application.Services
{
    public class VoterServices : IVoterServices
    {
        private readonly IVoterRepository _voterRepository;
        private readonly IUnitOfWork _unitOfWork;
        public VoterServices(IVoterRepository voterRepository, IUnitOfWork unitOfWork)
        {
            _voterRepository = voterRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<VoterResponseDTO>> AddVoterAsync(CreateVoterRequestDTO request)
        {
            Voter voter = request.ToVoter();
            Voter createdVoter = await _voterRepository.AddVoterAsync(voter);

            if (createdVoter == null)
            {
                return Result<VoterResponseDTO>.Failure(
                    Error.NullValue);
            }

            if (createdVoter.Voted == false)
            {
                return Result<VoterResponseDTO>.Failure(Error.NullValue);
            }
            await _unitOfWork.SaveChangesAsync();

            var response = createdVoter.ToVoterResponse();
            return Result<VoterResponseDTO>.Success(response);
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
