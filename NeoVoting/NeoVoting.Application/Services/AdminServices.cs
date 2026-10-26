using Microsoft.Extensions.Logging;
using NeoVoting.Application.RequestDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IElectionRepository _electionRepository;
        private readonly IPollRepository _pollRepository;
        private readonly ISystemAuditLogRepository _systemAuditLogRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly ILogger<AdminServices> _logger;

        public AdminServices(IUnitOfWork unitOfWork, IElectionRepository electionRepository, IPollRepository pollRepository, ISystemAuditLogRepository systemAuditLogRepository,
            ICurrentUserServices currentUserServices, ILogger<AdminServices> logger)
        {
            _unitOfWork = unitOfWork;
            _electionRepository = electionRepository;
            _pollRepository = pollRepository;
            _systemAuditLogRepository = systemAuditLogRepository;
            _currentUserServices = currentUserServices;
            _logger = logger;
        }

        public async Task<Result<Election_ResponseDTO>> CompleteElectionAsync(int electionId)
        {
            _logger.LogInformation("Completing election {ElectionId}.", electionId);

            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<Election_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));
            }

            if (election.Status != StatusEnum.Voting)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.ElectionInvalidState), "Election is not in the voting phase and cannot be completed."));
            }

            election.EndVotingPhase();

            await _unitOfWork.SaveChangesAsync();

            var responseDTO = election.ToElectionResponse();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string username = _currentUserServices.UserName!;

            var logEntry = SystemAuditLog.Create(adminId, username, SystemActionTypesEnum.ADMIN_ENDED_ELECTION_VOTING_PHASE);

            _systemAuditLogRepository.Add(logEntry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Election {ElectionId} completed successfully.", electionId);
            return Result<Election_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<Poll_ResponseDTO>> CompletePollAsync(int pollId)
        {
            _logger.LogInformation("Completing poll {PollId}.", pollId);

            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Poll_NotFound), "Poll not found."));
            }

            if (poll.Status != StatusEnum.Voting)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollInvalidState), "Poll is not in the voting phase and cannot be completed."));
            }

            poll.EndPoll();

            await _unitOfWork.SaveChangesAsync();

            var responseDTO = poll.ToPollResponse();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string username = _currentUserServices.UserName!;

            var logEntry = SystemAuditLog.Create(adminId, username, SystemActionTypesEnum.ADMIN_ENDED_POLL);

            _systemAuditLogRepository.Add(logEntry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Poll {PollId} completed successfully.", pollId);
            return Result<Poll_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<Election_ResponseDTO>> CreateElectionAsync(ElectionCreate_RequestDTO dto)
        {
            _logger.LogInformation("Creating election with name {ElectionName}.", dto.Name);

            if(await _electionRepository.IsActiveElectionExistsAsync())
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.ElectionAlreadyActive),"An active election already exists. Cannot create a new election."));
            }
            if(await _pollRepository.IsActivePollExistsAsync())
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollAlreadyActive), "An active poll already exists. Cannot create a new election."));
            }

            if(await _electionRepository.IsElectionNameExistsAsync(dto.Name!))
            {
                return Result<Election_ResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Election_DuplicateName), "An election with the same name already exists. Cannot create a new election."));
            }

            var election = Election.Create(dto.Name!,dto.NominationStartDate!.Value,                dto.NominationEndDate!.Value,
                dto.VotingStartDate!.Value, dto.VotingEndDate!.Value);

            _electionRepository.Add(election);

            await _unitOfWork.SaveChangesAsync();

            var addedElection = await _electionRepository.GetByIdAsync(election.Id);
            if(addedElection == null)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Failed to retrieve the created election."));
            }
            var responseDTO = addedElection.ToElectionResponse();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string username = _currentUserServices.UserName!;

            var logEntry = SystemAuditLog.Create(adminId, username, SystemActionTypesEnum.ADMIN_CREATED_ELECTION);

            _systemAuditLogRepository.Add(logEntry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Election {ElectionId} created successfully.", election.Id);
            return Result<Election_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<Poll_ResponseDTO>> CreatePollAsync(PollCreate_RequestDTO dto)
        {
            _logger.LogInformation("Creating poll with name {PollName}.", dto.Name);

            // 1. Check count (min 2, max 5)
            if (dto.Answers == null || dto.Answers.Count < 2 || dto.Answers.Count > 5)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollInvalidAnswers), "A poll must have between 2 and 5 answers."));
            }

            // 2. Check for identical string duplicates
            if (dto.Answers.Distinct().Count() != dto.Answers.Count)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollInvalidAnswers), "Poll answers cannot contain identical duplicate strings."));
            }


            if (await _pollRepository.IsActivePollExistsAsync())
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollAlreadyActive), "An active poll already exists. Cannot create a new poll."));
            }
            if (await _electionRepository.IsActiveElectionExistsAsync())
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.ElectionAlreadyActive), "An active election already exists. Cannot create a new poll."));
            }

            if (await _pollRepository.IsPollNameExistsAsync(dto.Name!))
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Conflict(nameof(ProblemDetails409ErrorTypes.Poll_DuplicateName), "An poll with the same name already exists. Cannot create a new poll."));
            }

            var poll = Poll.Create(dto.Name!, dto.Question! ,dto.StartDate!.Value, dto.EndDate!.Value);

            foreach (var answer in dto.Answers!)
            {
                poll.PollAnswers.Add(PollAnswer.Create(answer));
            }

            _pollRepository.Add(poll);

            await _unitOfWork.SaveChangesAsync();

            var addedPoll = await _pollRepository.GetByIdAsync(poll.Id);
            if (addedPoll == null)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Failure(nameof(ProblemDetails500ErrorTypes.Server_Error), "Failed to retrieve the created poll."));
            }
            var responseDTO = addedPoll.ToPollResponse();


            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string username = _currentUserServices.UserName!;

            var logEntry = SystemAuditLog.Create(adminId, username, SystemActionTypesEnum.ADMIN_CREATED_POLL);

            _systemAuditLogRepository.Add(logEntry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Poll {PollId} created successfully.", poll.Id);
            return Result<Poll_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<PagedResult<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsAsync(SystemActionTypesEnum? actionTypesEnum, int? adminId, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Retrieving paged system audit logs with action filter {ActionType}, admin filter {AdminId}, page {PageNumber}, size {PageSize}.", actionTypesEnum, adminId, pageNumber, pageSize);

            if (pageNumber < 1)
            {
                _logger.LogWarning("Paged system audit log request rejected because page number {PageNumber} is invalid.", pageNumber);
                return Result<PagedResult<SystemAuditLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageNumber must be greater than 0."));
            }
            if (pageSize < 1)
            {
                _logger.LogWarning("Paged system audit log request rejected because page size {PageSize} is invalid.", pageSize);
                return Result<PagedResult<SystemAuditLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageSize must be greater than 0."));
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            // 3. Get total count
            int totalCount = await _systemAuditLogRepository.CountAsync(actionTypesEnum, adminId);

            var pagedLogs = await _systemAuditLogRepository.GetPagedAsync(actionTypesEnum, adminId, pageNumber, pageSize);


            var responseDTOs = pagedLogs.Select(log => log.ToSystemAuditLogResponse()).ToList();
            var pagedResult = new PagedResult<SystemAuditLog_ResponseDTO>
            {
                Data = responseDTOs,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            _logger.LogInformation("Paged system audit logs retrieved successfully for action filter {ActionType}, admin filter {AdminId}.", actionTypesEnum, adminId);
            return Result<PagedResult<SystemAuditLog_ResponseDTO>>.Success(pagedResult);

        }

        public async Task<Result<Election_ResponseDTO>> StartElectionAsync(int electionId)
        {
            _logger.LogInformation("Starting election {ElectionId}.", electionId);

            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                _logger.LogWarning("Attempted to start election {ElectionId}, but it was not found.", electionId);
                return Result<Election_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));
            }

            if(election.Status != StatusEnum.Upcoming)
            {
                _logger.LogWarning("Election {ElectionId} cannot be started because its status is {CurrentStatus}.", electionId, election.Status);
                return Result<Election_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.ElectionInvalidState), "Election is not in the upcoming phase and cannot be started."));
            }

            election.StartVotingPhase();

            await _unitOfWork.SaveChangesAsync();

            var responseDTO = election.ToElectionResponse();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string username = _currentUserServices.UserName!;

            var logEntry = SystemAuditLog.Create(adminId, username, SystemActionTypesEnum.ADMIN_STARTED_ELECTION_VOTING_PHASE);

            _systemAuditLogRepository.Add(logEntry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Election {ElectionId} started successfully.", electionId);
            return Result<Election_ResponseDTO>.Success(responseDTO);

        }

        public async Task<Result<Poll_ResponseDTO>> StartPollAsync(int pollId)
        {
            _logger.LogInformation("Starting poll {PollId}.", pollId);

            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                _logger.LogWarning("Attempted to start poll {PollId}, but it was not found.", pollId);
                return Result<Poll_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Poll_NotFound), "Poll not found."));
            }

            if (poll.Status != StatusEnum.Upcoming)
            {
                _logger.LogWarning("Poll {PollId} cannot be started because its status is {CurrentStatus}.", pollId, poll.Status);
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(nameof(ProblemDetails400ErrorTypes.PollInvalidState), "Poll is not in the upcoming phase and cannot be started."));
            }

            poll.StartPoll();

            await _unitOfWork.SaveChangesAsync();

            var responseDTO = poll.ToPollResponse();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string username = _currentUserServices.UserName!;

            var logEntry = SystemAuditLog.Create(adminId, username, SystemActionTypesEnum.ADMIN_STARTED_POLL);

            _systemAuditLogRepository.Add(logEntry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Poll {PollId} started successfully.", pollId);
            return Result<Poll_ResponseDTO>.Success(responseDTO);
        }
    }
}