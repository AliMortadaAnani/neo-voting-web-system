using NeoVoting.Application.AuthDTOs;
using NeoVoting.Application.NeoVotingDTOs;
using NeoVoting.Application.RequestDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Contracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.ErrorHandling;
using NeoVoting.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Services
{
    public class AdminServices : IAdminServices
    {   
        private readonly IElectionRepository _electionRepository;
        private readonly ISystemAuditLogRepository _systemAuditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGovernmentSystemGateway _governmentSystemGateway;

        public AdminServices(IElectionRepository electionRepository, ISystemAuditLogRepository systemAuditLogRepository,IUnitOfWork unitOfWork,IGovernmentSystemGateway governmentSystemGateway)
        {
            _electionRepository = electionRepository;
            _systemAuditLogRepository = systemAuditLogRepository;
            _unitOfWork = unitOfWork;
            _governmentSystemGateway = governmentSystemGateway;
        }

        private Election_ResponseDTO MapToResponseDTO(Election election)
        {
            return new Election_ResponseDTO
            {
                Id = election.Id,
                Name = election.Name,
                NominationStartDate = election.NominationStartDate,
                NominationEndDate = election.NominationEndDate,
                VotingStartDate = election.VotingStartDate,
                VotingEndDate = election.VotingEndDate,
                ElectionStatusId = election.ElectionStatusId,
                ElectionStatusName = election.ElectionStatus.Name
            };
        }
        private SystemAuditLog_ResponseDTO MapToResponseDTO(SystemAuditLog log)
        {
            return new SystemAuditLog_ResponseDTO
            {
                Id = log.Id,
                TimestampUTC = log.TimestampUTC,
                ActionType = log.ActionType,
                Details = log.Details,
                UserId = log.UserId,
                Username = log.Username,
                ElectionName = log.ElectionName,
                CandidateProfileId = log.CandidateProfileId,
                ElectionId = log.ElectionId
            };
        }

        public async Task<Result<Election_ResponseDTO>> AddElectionAsync(ElectionAdd_RequestDTO requestDTO, CancellationToken cancellationToken)
        {   
            var activeElectionExists = await _electionRepository.IsActiveElectionExistsAsync(cancellationToken);
            
            if(activeElectionExists)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Election_AlreadyActive),
                    "An active election already exists. Cannot create a new election while another is active."));
            }

           
            var resetVotedFlagResult = await _governmentSystemGateway.ResetAllVotersVoteStatusAsync(cancellationToken);

            // If the network failed, or the API returned 404/400/500, we stop here.
            if (resetVotedFlagResult.IsFailure)
            {
                return Result<Election_ResponseDTO>.Failure(resetVotedFlagResult.Error);
            }

            var isReset = resetVotedFlagResult.Value; // This contains Name, DOB, Eligibility, etc.

            if (!isReset)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.GovernmentSystemGateway_Error),
                    "Failed to reset voters' voted status via the government system."));
            }

            Election election = requestDTO.ToElection();
            
            var addedElection = await _electionRepository.AddElectionAsync(election, cancellationToken);

            if(addedElection == null)
            {

                return Result<Election_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Election_CreationFailed),
                    "Failed to create election.") );
                    
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Election_ResponseDTO>.Success(MapToResponseDTO(election));


        }

        public async Task<Result<Election_ResponseDTO>> UpdateElectionStatusAsync(Guid electionId, ElectionStatusEnum newStatus, CancellationToken cancellationToken)
        {
            var election = await _electionRepository.GetElectionByIdAsync(electionId, cancellationToken);

            if (election == null)
            {
                return Result<Election_ResponseDTO>.Failure(Error.NotFound(
                    nameof(ProblemDetails404ErrorTypes.Election_NotFound),
                    $"Election with ID '{electionId}' was not found."));
            }

            var currentStatus = (ElectionStatusEnum)election.ElectionStatusId;

            if (currentStatus == newStatus)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Election_AlreadyInStatus),
                    $"Election is already in '{newStatus}' status."));
            }

            try
            {
                // Use the appropriate factory method based on the target status
                switch (newStatus)
                {
                    case ElectionStatusEnum.Nomination:
                        election.StartNominationPhase();
                        break;
                    case ElectionStatusEnum.PreVotingPhase:
                        election.StartPreVotingPhase();
                        break;
                    case ElectionStatusEnum.Voting:
                        election.StartVotingPhase();
                        break;
                    case ElectionStatusEnum.Completed:
                        {
                            election.CompleteElection();
                            //insert winners and statistics logic here


                        }
                        break;
                    case ElectionStatusEnum.Upcoming:
                        return Result<Election_ResponseDTO>.Failure(Error.Validation(
                            nameof(ProblemDetails400ErrorTypes.Election_InvalidStatusTransition),
                            "Cannot transition an election back to 'Upcoming' status."));
                    default:
                        return Result<Election_ResponseDTO>.Failure(Error.Validation(
                            nameof(ProblemDetails400ErrorTypes.Election_InvalidStatusTransition),
                            $"Invalid election status: '{newStatus}'."));
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Election_InvalidStatusTransition),
                    ex.Message));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Election_ResponseDTO>.Success(MapToResponseDTO(election));
        }

        public async Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            // 1. VALIDATION (Must be first)
            if (pageNumber < 1)
            {
                return Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageNumber must be greater than 0."));
            }

            if (pageSize < 1)
            {
                return Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Paging_InvalidInput), "PageSize must be greater than 0."));
            }

            // 2. SECURITY: Cap the PageSize
            if (pageSize > 100) pageSize = 100;

            // 3. CALCULATION
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;

            // 4. DATA RETRIEVAL (Sequential due to EF Core DbContext)
            var auditLogs = await _systemAuditLogRepository.GetPagedSystemAuditLogsAsync(skip, take, cancellationToken);
            var totalCount = await _systemAuditLogRepository.GetCountOfTotalSystemAuditLogsAsync(cancellationToken);

            // 5. HANDLE EMPTY RESULTS
            if (auditLogs.Count == 0 && pageNumber == 1)
            {
                // It's not an error if the DB is empty, just return empty response
                return Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>.Success(new List<SystemAuditLog_ResponseDTO>());
            }

            if (auditLogs.Count == 0 && totalCount > 0)
            {
                return Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>.Failure(
                    Error.NotFound(nameof(ProblemDetails404ErrorTypes.Paging_OutOfBounds), "Page number exceeds total pages."));
            }

            // 6. MAP TO RESPONSE DTOs
            var response = auditLogs.Select(MapToResponseDTO).ToList();

            return Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>.Success(response);
        }

        public Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsByActionTypeAsync(SystemActionTypesEnum systemActionTypesEnum, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsByElectionIdAsync(Guid electionId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<SystemAuditLog_ResponseDTO>>> GetSystemAuditLogsByUserIdAsync(Guid userID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
