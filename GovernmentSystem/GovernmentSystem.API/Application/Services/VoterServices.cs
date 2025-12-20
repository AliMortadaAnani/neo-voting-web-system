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

            var voterAdded = await _voterRepository.AddVoterAsync(voter);

            if (voterAdded == null)
            {
                return Result<VoterResponseDTO>.Failure(Error.Failure("Voter.AdditionFailed", "Voter could not be added."));
            }

            await _unitOfWork.SaveChangesAsync();

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

            await _unitOfWork.SaveChangesAsync();

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

            await _unitOfWork.SaveChangesAsync();

            return Result<VoterResponseDTO>.Success(voter.ToVoterResponse());
        }

        public async Task<Result<List<VoterResponseDTO>>> GetAllVotersAsync()
        {
            var voters = await _voterRepository.GetAllVotersAsync();

            if (voters.Count == 0)
            {
                return Result<List<VoterResponseDTO>>.Failure(Error.NotFound("Voters.Missing", "No voters found."));
            }

            var response = voters.Select(v => v.ToVoterResponse()).ToList();
            return Result<List<VoterResponseDTO>>.Success(response);
        }

        public async Task<Result<List<VoterResponseDTO>>> GetPaginatedVotersAsync(int pageNumber, int pageSize)
        {
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                return Result<List<VoterResponseDTO>>.Failure(
                    Error.Validation("Paging.Invalid", "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                return Result<List<VoterResponseDTO>>.Failure(
                    Error.Validation("Paging.Invalid", "PageSize must be greater than 0."));
            }

            // 2. SECURITY: Cap the PageSize
            // If they ask for 5000, force it down to 100 to protect RAM/Network.
            if (pageSize > 100) pageSize = 100;
            if (pageSize < 1) pageSize = 20; // Default safety

            // 3. CALCULATION
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            // 4. DATA RETRIEVAL (Parallel Execution for Speed)
            // We need both the Data (Page) and the Count (Total)
            var votersTask = await _voterRepository.GetPagedVotersStoredProcAsync(skip, take);

            var countTask = await _voterRepository.GetTotalVotersCountAsync(); // You need to add this Repo method
                                                                               //await Task.WhenAll(votersTask, countTask);
            /*
             * Summary
            If you use new SqlConnection(): You can use Task.WhenAll (Parallel).
            If you use _dbContext: You must use await ...; await ...; (Sequential).
            Since you requested to use EF Core (FromSqlRaw) for consistency,
            you must accept the trade-off of executing them sequentially.
             */

            var votersFromPagedVoters = votersTask;  //votersTask.Result;
            var totalCountOfVotersInDB = countTask; //countTask.Result;

            // 5. HANDLE EMPTY RESULTS
            if (votersFromPagedVoters.Count == 0 && pageNumber == 1)
            {
                // It's not an error if the DB is empty, just return empty response
                return Result<List<VoterResponseDTO>>.Success(new List<VoterResponseDTO>());
            }
            // If they ask for Page 100 but we only have 5 pages
            if (votersFromPagedVoters.Count == 0 && totalCountOfVotersInDB > 0)
            {
                return Result<List<VoterResponseDTO>>.Failure(
                    Error.NotFound("Paging.OutOfBounds", "Page number exceeds total pages."));
            }

            var response = votersFromPagedVoters.Select(v => v.ToVoterResponse()).ToList();
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

            // If the admin clicks save without changing data, it should still say Success.
            await _unitOfWork.SaveChangesAsync();

            return Result<VoterResponseDTO>.Success(voter.ToVoterResponse());
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

        // NeoVoting Specific Services
        public async Task<Result<NeoVoting_VoterResponseDTO>> UpdateVoterIsRegisteredToTrueAsync(NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound("Voter.NotFound", "Voter with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!voter.EligibleForElection)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.NotValid", "Voter with this nationalId was not authorized.Please contact Government System for support."));
            }
            if (voter.VotingToken != request.VotingToken!.Value)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.UnauthorizedToken", "Voter with this nationalId and this voting token was not authorized.Please enter your voting token correctly or contact Government System for support."));
            }
            if (!voter.ValidToken)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.UnauthorizedToken", "Voter with this nationalId and this voting token was not authorized.Please contact Government System for support."));
            }

            if (voter.IsRegistered)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Conflict("Voter.AlreadyRegistered", "Voter with this nationalId and this voting token was already registered.You cannot register with a new account."));
            }
            if (string.IsNullOrEmpty(request.RegisteredUsername))
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Validation("Voter.InvalidUsername", "Username cannot be null or empty when registering as voter in NeoVoting."));
            }
            voter.MarkVoterAsRegisteredWithNewRegisteredUsername(request.RegisteredUsername);
            _voterRepository.Update(voter);

            await _unitOfWork.SaveChangesAsync();

            return Result<NeoVoting_VoterResponseDTO>.Success(voter.ToNeoVoting_VoterResponse());
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> UpdateHasVotedToTrueAsync(NeoVoting_VoterHasVotedRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound("Voter.NotFound", "Voter with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!voter.EligibleForElection)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.NotValid", "Voter with this nationalId was not authorized.Please contact Government System for support."));
            }
            if (voter.VotingToken != request.VotingToken!.Value)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.UnauthorizedToken", "Voter with this nationalId and this voting token was not authorized.Please enter your voting token correctly or contact Government System for support."));
            }
            if (!voter.ValidToken)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.UnauthorizedToken", "Voter with this nationalId and this voting token was not authorized.Please contact Government System for support."));
            }

            if (!voter.IsRegistered)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.NotRegistered", "Voter with this nationalId and this voting token was not registered.You cannot vote without registering an account."));
            }

            if (voter.Voted)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Conflict("Voter.AlreadyVoted", "Voter with this nationalId and this voting token had already casted a vote in this election.You cannot vote!"));
            }
            voter.MarkVoterAsVoted();
            _voterRepository.Update(voter);

            await _unitOfWork.SaveChangesAsync();

            return Result<NeoVoting_VoterResponseDTO>.Success(voter.ToNeoVoting_VoterResponse());
        }

        public async Task<Result<NeoVoting_VoterResponseDTO>> GetVoterForNeoVotingAsync(NeoVoting_GetVoterRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);
            if (voter == null)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound("Voter.NotFound", "Voter with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!voter.EligibleForElection)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.NotValid", "Voter with this nationalId was not authorized.Please contact Government System for support."));
            }
            if (voter.VotingToken != request.VotingToken!.Value)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.UnauthorizedToken", "Voter with this nationalId and this voting token was not authorized.Please enter your voting token correctly or contact Government System for support."));
            }
            if (!voter.ValidToken)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized("Voter.UnauthorizedToken", "Voter with this nationalId and this voting token was not authorized.Please contact Government System for support."));
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