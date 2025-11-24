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
                return Result<VoterResponseDTO>.Failure(Error.Failure("Voter.AdditionFailed","Voter could not be added"));
            }
            

            int rowsAdded = await _unitOfWork.SaveChangesAsync();

            bool isAdded = rowsAdded > 0;

            if (isAdded == false)
            {
                return Result<VoterResponseDTO>.Failure(Error.Failure("Voter.AdditionFailed", "Voter could not be added."));
            }
            var response = createdVoter.ToVoterResponse();
            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteByNationalIdAsync(DeleteVoterRequestDTO request)
        {

            // 1. CHECK: Does it exist?
            // We use the Repo to find it first.
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                // 2. FAILURE: Return 404 Not Found
                // This answers your question: "How can we check for success?"
                return Result<bool>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }

            // 3. ACTION: Tell Repo to delete this specific object
            _voterRepository.Delete(voter);

            // 4. COMMIT: Unit of Work saves changes to DB
            int rowsDeleted = await _unitOfWork.SaveChangesAsync();

            bool isDeleted = rowsDeleted > 0;   
            // 5. SUCCESS: Return True
            if(isDeleted == false)
            {
                return Result<bool>.Failure(Error.Failure("Voter.DeletionFailed", "Voter could not be deleted."));
            }
            return Result<bool>.Success(isDeleted);
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

        public async Task<Result<VoterResponseDTO>> UpdateByNationalIdAsync(UpdateVoterRequestDTO request)
        {
            
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
               
                return Result<VoterResponseDTO>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }

            voter.UpdateDetails(
               request.GovernorateId!.Value,
               request.FirstName!,
               request.LastName!,
               request.DateOfBirth!.Value,
               request.Gender!.Value,
               request.EligibleForElection!.Value,
               request.ValidToken!.Value,
               request.IsRegistered!.Value,
               request.Voted!.Value
               );

            _voterRepository.Update(voter);

            
            int rowsUpdated = await _unitOfWork.SaveChangesAsync();

            bool isUpdated = rowsUpdated > 0;
            
            if (isUpdated == false)
            {
                return Result<VoterResponseDTO>.Failure(Error.Failure("Voter.UpdateFailed", "Voter could not be updated."));
            }
           

            var response = voter.ToVoterResponse();
            return Result<VoterResponseDTO>.Success(response);
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
