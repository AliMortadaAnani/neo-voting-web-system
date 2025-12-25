using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Contracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.Services
{
    public class NeoVotingServices : INeoVotingServices
    {
        private readonly IVoterRepository _voterRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;
        public NeoVotingServices(IVoterRepository voterRepository, ICandidateRepository candidateRepository, IUnitOfWork unitOfWork)
        {
            _voterRepository = voterRepository;
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
        }

        // NeoVoting Voter Specific Services
        public async Task<Result<NeoVoting_VoterResponseDTO>> UpdateVoterIsRegisteredToTrueAsync(NeoVoting_VoterIsRegisteredRequestDTO request)
        {
            var voter = await _voterRepository.GetVoterByNationalIdAsync(request.NationalId!.Value);

            if (voter == null)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!voter.EligibleForElection)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_NotEligible), "Voter with this nationalId was not authorized.Please contact Government System for support."));
            }
            if (voter.VotingToken != request.VotingToken!.Value)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_InvalidToken), "Voter with this nationalId and this voting token was not authorized.Please enter your voting token correctly or contact Government System for support."));
            }
            if (!voter.ValidToken)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_InvalidToken), "Voter with this nationalId and this voting token was not authorized.Please contact Government System for support."));
            }

            if (voter.IsRegistered)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyRegistered), "Voter with this nationalId and this voting token was already registered.You cannot register with a new account."));
            }
            if (string.IsNullOrEmpty(request.RegisteredUsername))
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.Voter_InvalidUsername), "Username cannot be null or empty when registering as voter in NeoVoting."));
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
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!voter.EligibleForElection)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_NotEligible), "Voter with this nationalId was not authorized.Please contact Government System for support."));
            }
            if (voter.VotingToken != request.VotingToken!.Value)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_InvalidToken), "Voter with this nationalId and this voting token was not authorized.Please enter your voting token correctly or contact Government System for support."));
            }
            if (!voter.ValidToken)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_InvalidToken), "Voter with this nationalId and this voting token was not authorized.Please contact Government System for support."));
            }

            if (!voter.IsRegistered)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_NotRegistered), "Voter with this nationalId and this voting token was not registered.You cannot vote without registering an account."));
            }

            if (voter.Voted)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Voter_AlreadyVoted), "Voter with this nationalId and this voting token had already casted a vote in this election.You cannot vote!"));
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
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Voter_NotFound), "Voter with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!voter.EligibleForElection)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_NotEligible), "Voter with this nationalId was not authorized.Please contact Government System for support."));
            }
            if (voter.VotingToken != request.VotingToken!.Value)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_InvalidToken), "Voter with this nationalId and this voting token was not authorized.Please enter your voting token correctly or contact Government System for support."));
            }
            if (!voter.ValidToken)
            {
                return Result<NeoVoting_VoterResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Voter_InvalidToken), "Voter with this nationalId and this voting token was not authorized.Please contact Government System for support."));
            }

            var response = voter.ToNeoVoting_VoterResponse();
            return Result<NeoVoting_VoterResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> ResetAllVotedAsFalseAsync()
        {
            await _voterRepository.ResetAllVotedFieldToFalse();
            return Result<bool>.Success(true);
        }


        // NeoVoting Candidate Specific Services
        public async Task<Result<NeoVoting_CandidateResponseDTO>> GetCandidateForNeoVotingAsync(NeoVoting_GetCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!candidate.EligibleForElection)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Candidate_NotEligible), "Candidate with this nationalId is not eligible for election. Please contact Government System for support."));
            }
            if (candidate.NominationToken != request.NominationToken!.Value)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Candidate_InvalidToken), "Candidate with this nationalId and this nomination token was not authorized. Please enter your nomination token correctly or contact Government System for support."));
            }
            if (!candidate.ValidToken)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Candidate_InvalidToken), "Candidate with this nationalId and this nomination token was not authorized. Please contact Government System for support."));
            }

            return Result<NeoVoting_CandidateResponseDTO>.Success(candidate.ToNeoVoting_CandidateResponse());
        }

        public async Task<Result<NeoVoting_CandidateResponseDTO>> UpdateCandidateIsRegisteredToTrueAsync(NeoVoting_CandidateIsRegisteredRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate with this nationalId was not found.Please enter your nationalId correctly or contact Government System for support."));
            }
            if (!candidate.EligibleForElection)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Candidate_NotEligible), "Candidate with this nationalId is not eligible for election. Please contact Government System for support."));
            }
            if (candidate.NominationToken != request.NominationToken!.Value)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Candidate_InvalidToken), "Candidate with this nationalId and this nomination token was not authorized. Please enter your nomination token correctly or contact Government System for support."));
            }
            if (!candidate.ValidToken)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Unauthorized(nameof(ProblemDetails401ErrorTypes.Candidate_InvalidToken), "Candidate with this nationalId and this nomination token was not authorized. Please contact Government System for support."));
            }
            if (candidate.IsRegistered)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Candidate_AlreadyRegistered), "Candidate with this nationalId was already registered. You cannot register with a new account."));
            }
            if (string.IsNullOrEmpty(request.RegisteredUsername))
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.Candidate_InvalidUsername), "Username cannot be null or empty when registering as candidate in NeoVoting."));
            }
            candidate.MarkCandidateAsRegisteredWithNewRegisteredUsername(request.RegisteredUsername);
            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
            var response = candidate.ToNeoVoting_CandidateResponse();
            return Result<NeoVoting_CandidateResponseDTO>.Success(response);
        }
    }
}
