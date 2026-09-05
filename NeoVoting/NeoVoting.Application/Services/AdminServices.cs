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

        public AdminServices(IUnitOfWork unitOfWork, IElectionRepository electionRepository, IPollRepository pollRepository, ISystemAuditLogRepository systemAuditLogRepository,
            ICurrentUserServices currentUserServices)
        {
            _unitOfWork = unitOfWork;
            _electionRepository = electionRepository;
            _pollRepository = pollRepository;
            _systemAuditLogRepository = systemAuditLogRepository;
            _currentUserServices = currentUserServices;
        }

        public async Task<Result<Election_ResponseDTO>> CompleteElectionAsync(int electionId)
        {
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

            return Result<Election_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<Poll_ResponseDTO>> CompletePollAsync(int pollId)
        {
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

            return Result<Poll_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<Election_ResponseDTO>> CreateElectionAsync(ElectionCreate_RequestDTO dto)
        {
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

            return Result<Election_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<Poll_ResponseDTO>> CreatePollAsync(PollCreate_RequestDTO dto)
        {

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

            return Result<Poll_ResponseDTO>.Success(responseDTO);
        }

        public async Task<Result<PagedResult<SystemAuditLog_ResponseDTO>>> GetPagedSystemAuditLogsAsync(SystemActionTypesEnum? actionTypesEnum, int? adminId, int pageNumber, int pageSize)
        {

            if (pageNumber < 1)
            {
                return Result<PagedResult<SystemAuditLog_ResponseDTO>>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.PagingInvalidParameters), "PageNumber must be greater than 0."));
            }
            if (pageSize < 1)
            {
             
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

            return Result<PagedResult<SystemAuditLog_ResponseDTO>>.Success(pagedResult);

        }

        public async Task<Result<Election_ResponseDTO>> StartElectionAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null) { 
            return Result<Election_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Election_NotFound), "Election not found."));
            }

            if(election.Status != StatusEnum.Upcoming)
            {
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

            return Result<Election_ResponseDTO>.Success(responseDTO);

        }

        public async Task<Result<Poll_ResponseDTO>> StartPollAsync(int pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.NotFound(nameof(ProblemDetails404ErrorTypes.Poll_NotFound), "Poll not found."));
            }

            if (poll.Status != StatusEnum.Upcoming)
            {
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

            return Result<Poll_ResponseDTO>.Success(responseDTO);
        }
    }
}