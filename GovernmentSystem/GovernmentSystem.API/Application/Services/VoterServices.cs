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

            // Note: AddAsync inside repo usually just tracks the entity.
            await _voterRepository.AddVoterAsync(voter);

            int rowsAdded = await _unitOfWork.SaveChangesAsync();

            // For ADD, checking > 0 is fine because a new row MUST be created.
            if (rowsAdded == 0)
            {
                return Result<VoterResponseDTO>.Failure(Error.Failure("Voter.AdditionFailed", "Voter could not be added."));
            }

            return Result<VoterResponseDTO>.Success(voter.ToVoterResponse());
        }

        public async Task<Result<bool>> DeleteByNationalIdAsync(DeleteVoterRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<bool>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }

            _voterRepository.Delete(voter);

            int rowsDeleted = await _unitOfWork.SaveChangesAsync();

            // For DELETE, checking > 0 is fine.
            if (rowsDeleted == 0)
            {
                return Result<bool>.Failure(Error.Failure("Voter.DeletionFailed", "Voter could not be deleted."));
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<VoterResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenVoterRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<VoterResponseDTO>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }

            // Domain logic
            voter.GenerateNewVotingToken();
            _voterRepository.Update(voter);

            // FIX: Don't fail if 0 rows updated (though for a new GUID token, it should always update)
            await _unitOfWork.SaveChangesAsync();

            return Result<VoterResponseDTO>.Success(voter.ToVoterResponse());
        }

        public async Task<Result<List<VoterResponseDTO>>> GetAllVotersAsync()
        {
            // FIX: Await here!
            var voters = await _voterRepository.GetAllVotersAsync();

            if (voters.Count == 0)
            {
                return Result<List<VoterResponseDTO>>.Failure(Error.NotFound("Voters.Missing", "No voters found."));
            }

            var response = voters.Select(v => v.ToVoterResponse()).ToList();
            return Result<List<VoterResponseDTO>>.Success(response);
        }


        public async Task<Result<VoterResponseDTO>> UpdateByNationalIdAsync(UpdateVoterRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<VoterResponseDTO>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }

            // Update Entity State
            voter.UpdateDetails(
               (GovernorateId)request.GovernorateId!.Value,
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

            // FIX: We do NOT check rowsUpdated > 0 here. 
            // If the admin clicks save without changing data, it should still say Success.
            await _unitOfWork.SaveChangesAsync();

            return Result<VoterResponseDTO>.Success(voter.ToVoterResponse());
        }

        public async Task<Result<bool>> UpdateVoterIsRegisteredToTrueAsync(NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<bool>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }
            if (voter.VotingToken != request.VotingToken!.Value
              || !voter.ValidToken || !voter.EligibleForElection
                )
            {
                return Result<bool>.Failure(Error.Unauthorized("Voter.NotValid", "Invalid voter credentials."));
            }
            // Idempotency check (Optional optimization)
            if (voter.IsRegistered)
            {
                // Already done, just return success
                return Result<bool>.Success(true);
            }

            voter.MarkVoterAsRegistered();
            _voterRepository.Update(voter);

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateHasVotedToTrueAsync(NeoVoting_VoterHasVotedRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<bool>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }
            if (voter.VotingToken != request.VotingToken!.Value
              || !voter.IsRegistered ||!voter.ValidToken || !voter.EligibleForElection
                )
            {
                return Result<bool>.Failure(Error.Unauthorized("Voter.NotValid", "Invalid voter credentials."));
            }
            if (voter.Voted) return Result<bool>.Success(true);

            voter.MarkVoterAsVoted();
            _voterRepository.Update(voter);

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<VoterResponseDTO>> GetByNationalIdAsync(GetVoterRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);
            if (voter == null)
            {
                return Result<VoterResponseDTO>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }
            var response = voter.ToVoterResponse();
            return Result<VoterResponseDTO>.Success(response);
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> GetVoterForNeoVotingAsync(NeoVoting_GetVoterRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);
            if (voter == null)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound("Voter.Missing", "Voter not found."));
            }
            if (voter.VotingToken != request.VotingToken!.Value
             || !voter.ValidToken || !voter.EligibleForElection
                )
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.NotValid", "Invalid voter credentials."));
            }
            var response = voter.ToNeoVoting_VoterResponse();
            return Result<NeoVoting_VoterResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> ResetAllVotedAsFalseAsync()
        {
            await _voterRepository.ResetAllVotedFieldToFalse();
            return Result<bool>.Success(true);
        }

    }
}