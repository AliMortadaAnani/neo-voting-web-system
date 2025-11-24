using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Contracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.Services
{
    public class CandidateServices : ICandidateServices
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateServices(ICandidateRepository candidateRepository, IUnitOfWork unitOfWork)
        {
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CandidateResponseDTO>> AddCandidateAsync(CreateCandidateRequestDTO request)
        {
            Candidate candidate = request.ToCandidate();
            await _candidateRepository.AddCandidateAsync(candidate);
            int rowsAdded = await _unitOfWork.SaveChangesAsync();
            if (rowsAdded == 0)
            {
                return Result<CandidateResponseDTO>.Failure(Error.Failure("Candidate.AdditionFailed", "Candidate could not be added."));
            }
            return Result<CandidateResponseDTO>.Success(candidate.ToCandidateResponse());
        }

        public async Task<Result<bool>> DeleteByNationalIdAsync(DeleteCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<bool>.Failure(Error.NotFound("Candidate.Missing", "Candidate not found."));
            }
            _candidateRepository.Delete(candidate);
            int rowsDeleted = await _unitOfWork.SaveChangesAsync();
            if (rowsDeleted == 0)
            {
                return Result<bool>.Failure(Error.Failure("Candidate.DeletionFailed", "Candidate could not be deleted."));
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<CandidateResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound("Candidate.Missing", "Candidate not found."));
            }
            candidate.GenerateNewNominationToken();
            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
            return Result<CandidateResponseDTO>.Success(candidate.ToCandidateResponse());
        }

        public async Task<Result<List<CandidateResponseDTO>>> GetAllCandidatesAsync()
        {
            var candidates = await _candidateRepository.GetAllCandidatesAsync();
            if (candidates.Count == 0)
            {
                return Result<List<CandidateResponseDTO>>.Failure(Error.NotFound("Candidates.Missing", "No candidates found."));
            }
            var response = candidates.Select(c => c.ToCandidateResponse()).ToList();
            return Result<List<CandidateResponseDTO>>.Success(response);
        }

        public async Task<Result<CandidateResponseDTO>> GetByNationalIdAsync(GetCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound("Candidate.Missing", "Candidate not found."));
            }
            return Result<CandidateResponseDTO>.Success(candidate.ToCandidateResponse());
        }

        public async Task<Result<NeoVoting_CandidateResponseDTO>> GetCandidateForNeoVotingAsync(NeoVoting_GetCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.NotFound("Candidate.Missing", "Candidate not found."));
            }
            if (candidate.NominationToken != request.NominationToken!.Value
              || !candidate.ValidToken || !candidate.EligibleForElection
                )
            {
                return Result<NeoVoting_CandidateResponseDTO>.Failure(Error.Unauthorized("Candidate.NotValid", "Invalid candidate credentials."));
            }
            return Result<NeoVoting_CandidateResponseDTO>.Success(candidate.ToNeoVoting_CandidateResponse());
        }

        public async Task<Result<CandidateResponseDTO>> UpdateByNationalIdAsync(UpdateCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound("Candidate.Missing", "Candidate not found."));
            }
            candidate.UpdateDetails(
                (GovernorateId)request.GovernorateId!.Value,
                request.FirstName!,
                request.LastName!,
                request.DateOfBirth!.Value,
                request.Gender!.Value,
                request.EligibleForElection!.Value,
                request.ValidToken!.Value,
                request.IsRegistered!.Value
            );
            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
            return Result<CandidateResponseDTO>.Success(candidate.ToCandidateResponse());
        }

        public async Task<Result<bool>> UpdateCandidateIsRegisteredToTrueAsync(NeoVoting_CandidateIsRegisteredRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<bool>.Failure(Error.NotFound("Candidate.Missing", "Candidate not found."));
            }
            if (candidate.NominationToken != request.NominationToken!.Value
              || !candidate.ValidToken || !candidate.EligibleForElection
                )
            {
                return Result<bool>.Failure(Error.Unauthorized("Candidate.NotValid", "Invalid candidate credentials."));
            }
            if (candidate.IsRegistered)
            {
                return Result<bool>.Success(true);
            }
            candidate.MarkCandidateAsRegistered();
            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
