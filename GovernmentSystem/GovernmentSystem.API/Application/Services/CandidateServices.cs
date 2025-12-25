using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Application.ResponseDTOs;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Contracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.Services
{
    public class CandidateServices(ICandidateRepository candidateRepository, IUnitOfWork unitOfWork) : ICandidateServices
    {
        private readonly ICandidateRepository _candidateRepository = candidateRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CandidateResponseDTO>> AddCandidateAsync(CreateCandidateRequestDTO request)
        {
            Candidate candidate = request.ToCandidate();

            var candidateAdded = await _candidateRepository.AddCandidateAsync(candidate);

            if (candidateAdded == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Candidate_OperationFailed), "Candidate could not be added."));
            }

            await _unitOfWork.SaveChangesAsync();

            var response = candidate.ToCandidateResponse();

            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> DeleteByNationalIdAsync(DeleteCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<bool>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }
            _candidateRepository.Delete(candidate);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<CandidateResponseDTO>> GenerateNewTokenByNationalIdAsync(GenerateNewTokenCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }
            candidate.GenerateNewNominationToken();
            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();

            var response = candidate.ToCandidateResponse();
            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<List<CandidateResponseDTO>>> GetAllCandidatesAsync()
        {
            var candidates = await _candidateRepository.GetAllCandidatesAsync();
            if (candidates.Count == 0)
            {
                return Result<List<CandidateResponseDTO>>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "No candidates found."));
            }
            var response = candidates.Select(c => c.ToCandidateResponse()).ToList();
            return Result<List<CandidateResponseDTO>>.Success(response);
        }

        public async Task<Result<List<CandidateResponseDTO>>> GetPaginatedCandidatesAsync(int pageNumber, int pageSize)
        {
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                return Result<List<CandidateResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }
            // 1. VALIDATION (Must be first)
            if (pageSize < 1)
            {
                return Result<List<CandidateResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageSize must be greater than 0."));
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
            var candidatesTask = _candidateRepository.GetPagedCandidatesStoredProcAsync(skip, take);
            var countTask = _candidateRepository.GetTotalCandidatesCountAsync(); // You need to add this Repo method
            await Task.WhenAll(candidatesTask, countTask);

            /*
            Task 1 (Stored Proc): Uses Connection A (Created manually).

            Task 2 (Count): Uses Connection B (Inside _dbContext).

            Result: They are two different cables going to the database.
            They can run at the same time. Task.WhenAll works.*/

            //Here we are waiting for both tasks to complete but from different contexts.
            //as candidatesTask is fetching data using ADO.NET and countTask is fetching data using Entity Framework Core.

            var candidates = candidatesTask.Result;
            var totalCount = countTask.Result;

            // 5. HANDLE EMPTY RESULTS
            if (candidates.Count == 0 && pageNumber == 1)
            {
                // It's not an error if the DB is empty, just return empty response
                return Result<List<CandidateResponseDTO>>.Success([]);
            }
            // If they ask for Page 100 but we only have 5 pages
            if (candidates.Count == 0 && totalCount > 0)
            {
                return Result<List<CandidateResponseDTO>>.Failure(
                    Error.NotFound(nameof(ProblemDetails404ErrorTypes.Paging_OutOfBounds), "Page number exceeds total pages."));
            }

            var response = candidates.Select(c => c.ToCandidateResponse()).ToList();
            return Result<List<CandidateResponseDTO>>.Success(response);
        }

        public async Task<Result<CandidateResponseDTO>> GetByNationalIdAsync(GetCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
            }
            var response = candidate.ToCandidateResponse();
            return Result<CandidateResponseDTO>.Success(response);
        }

        public async Task<Result<CandidateResponseDTO>> UpdateByNationalIdAsync(UpdateCandidateRequestDTO request)
        {
            var candidate = await _candidateRepository.GetCandidateByNationalIdAsync(request.NationalId!.Value);
            if (candidate == null)
            {
                return Result<CandidateResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Candidate_NotFound), "Candidate not found."));
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

            var response = candidate.ToCandidateResponse();
            return Result<CandidateResponseDTO>.Success(response);
        }

        
    }
}