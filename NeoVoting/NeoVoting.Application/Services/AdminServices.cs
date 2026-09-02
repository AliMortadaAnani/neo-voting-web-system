using NeoVoting.Application.RequestDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs;
using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;

namespace NeoVoting.Application.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IElectionRepository _electionRepository;
        private readonly IPollRepository _pollRepository;
        private readonly ISystemAuditLogRepository _systemAuditLogRepository;
        private readonly IElectionWinnerRepository _electionWinnerRepository;
        private readonly IPollAnswerRepository _pollAnswerRepository;
        private readonly IElectionStatisticsRepository _electionStatisticsRepository;
        private readonly IPollStatisticsRepository _pollStatisticsRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IUnitOfWork _unitOfWork;


        public AdminServices(IElectionRepository electionRepository, IPollRepository pollRepository, ISystemAuditLogRepository systemAuditLogRepository, IElectionWinnerRepository electionWinnerRepository, IPollAnswerRepository pollAnswerRepository, IElectionStatisticsRepository electionStatisticsRepository, IPollStatisticsRepository pollStatisticsRepository, IUnitOfWork unitOfWork , ICurrentUserServices currentUserServices)
        {
            _electionRepository = electionRepository;
            _pollRepository = pollRepository;
            _systemAuditLogRepository = systemAuditLogRepository;
            _electionWinnerRepository = electionWinnerRepository;
            _pollAnswerRepository = pollAnswerRepository;
            _electionStatisticsRepository = electionStatisticsRepository;
            _pollStatisticsRepository = pollStatisticsRepository;
            _unitOfWork = unitOfWork;
            _currentUserServices = currentUserServices;
        }


        public async Task<Result<Election_ResponseDTO>> CreateElectionAsync(ElectionCreate_RequestDTO dto)
        {
            if(await _electionRepository.IsActiveElectionExistsAsync())
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Active_Election_AlreadyExists) ,
                    "An active election already exists. Cannot create a new election while another is active."
                    ));
            }

            if(await _pollRepository.IsActivePollExistsAsync()) 
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Active_Poll_AlreadyExists) ,
                    "An active poll already exists. Cannot create a new election while another poll is active."
                    ));
            }

            if(await _electionRepository.IsElectionNameExistsAsync(dto.Name!))
            {
                return Result<Election_ResponseDTO>.Failure(Error.Conflict(
                    nameof(ProblemDetails409ErrorTypes.Election_DuplicateName) ,
                    "An election with this name already exists."
                    ));
            }

            Election election = Election.Create(
                dto.Name!,
                dto.NominationStartDate!.Value,
                dto.NominationEndDate!.Value,
                dto.VotingStartDate!.Value,
                dto.VotingEndDate!.Value
            );

            int adminId =(int)_currentUserServices.ApplicationUserId!;
            string adminUsername = _currentUserServices.UserName!;


            SystemAuditLog systemAuditLog = SystemAuditLog.Create(
                adminId,
                adminUsername,
                SystemActionTypesEnum.ADMIN_CREATED_ELECTION
            );

            _electionRepository.Add(election);
            _systemAuditLogRepository.Add(systemAuditLog);
            
            await _unitOfWork.SaveChangesAsync();

            var response = election.ToElectionResponse();

            return Result<Election_ResponseDTO>.Success(response);
            
        }

        public async Task<Result<Poll_ResponseDTO>> CreatePollAsync(PollCreate_RequestDTO dto)
        {
            if(await _pollRepository.IsActivePollExistsAsync())
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Active_Poll_AlreadyExists),
                    "An active poll already exists. Cannot create a new poll while another is active."
                    ));
            }

            if (await _electionRepository.IsActiveElectionExistsAsync())
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Active_Election_AlreadyExists),
                    "An active election already exists. Cannot create a new poll while another election is active."
                    ));
            }

            if (await _pollRepository.IsPollNameExistsAsync(dto.Name!))
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Conflict(
                    nameof(ProblemDetails409ErrorTypes.Poll_DuplicateName),
                    "A poll with this name already exists."
                    ));
            }

            Poll poll = Poll.Create(
                dto.Name!,
                dto.Question!,
                dto.StartDate!.Value,
                dto.EndDate!.Value
            );

            // 1. Check if the incoming list is null or empty
            if (dto.Answers == null || !dto.Answers.Any())
            {
                return Result<Poll_ResponseDTO>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Poll_InvalidAnswers),
                    "Answers list cannot be null or empty."));
            }

            // 2. Filter out whitespace/nulls, and normalize uniqueness (case-insensitive)
            var finalAnswersList = dto.Answers
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // 3. Check if the unique count falls outside the 2 to 5 boundary
            if (finalAnswersList.Count < 2 || finalAnswersList.Count > 5)
            {
                return Result<Poll_ResponseDTO>.Failure(
                    Error.Validation(nameof(ProblemDetails400ErrorTypes.Poll_InvalidAnswers),
                    "A poll must have between 2 and 5 unique answers."));
            }

            foreach (var answerText in finalAnswersList)
            {
                poll.PollAnswers.Add(PollAnswer.Create(answerText));
            }
  
            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string adminUsername = _currentUserServices.UserName!;

            SystemAuditLog systemAuditLog = SystemAuditLog.Create(
                adminId,
                adminUsername,
                SystemActionTypesEnum.ADMIN_CREATED_POLL
            );

            _pollRepository.Add(poll);
            _systemAuditLogRepository.Add(systemAuditLog);

            await _unitOfWork.SaveChangesAsync();

            var response = poll.ToPollResponse();

            return Result<Poll_ResponseDTO>.Success(response);
        }


        public async Task<Result<Election_ResponseDTO>> StartElectionAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<Election_ResponseDTO>.Failure(Error.NotFound(
                    nameof(ProblemDetails404ErrorTypes.Election_NotFound),
                    "Election not found."
                ));
            }

           if(election.Status == StatusEnum.Completed)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Start_CompletedElection),
                    "Election is already completed."
                ));
            }
            if (election.Status == StatusEnum.Voting)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Start_StartedElection),
                    "Election is already in the voting phase."
                ));
            }
            if(election.Status != StatusEnum.Upcoming)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Failure(
                    nameof(ProblemDetails500ErrorTypes.Server_Error),
                    "Election is in an unknown phase."
                ));
            }

            election.StartVotingPhase();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string adminUsername = _currentUserServices.UserName!;

            SystemAuditLog systemAuditLog = SystemAuditLog.Create(
                adminId,
                adminUsername,
                SystemActionTypesEnum.ADMIN_STARTED_ELECTION_VOTING_PHASE
            );

            _systemAuditLogRepository.Add(systemAuditLog);


            await _unitOfWork.SaveChangesAsync();

            var response = election.ToElectionResponse();

            return Result<Election_ResponseDTO>.Success(response);
        }

        public async Task<Result<Poll_ResponseDTO>> StartPollAsync(int pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.NotFound(
                    nameof(ProblemDetails404ErrorTypes.Poll_NotFound),
                    "Poll not found."
                ));
            }
            if(poll.Status == StatusEnum.Completed)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Start_CompletedPoll),
                    "Poll is already completed."
                ));
            }
            if (poll.Status == StatusEnum.Voting)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Start_StartedPoll),
                    "Poll is already in the voting phase."
                ));
            }
            if (poll.Status != StatusEnum.Upcoming)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Failure(
                    nameof(ProblemDetails500ErrorTypes.Server_Error),
                    "Poll is in an unknown phase."
                ));
            }

            poll.StartPoll();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string adminUsername = _currentUserServices.UserName!;

            SystemAuditLog systemAuditLog = SystemAuditLog.Create(
                adminId,
                adminUsername,
                SystemActionTypesEnum.ADMIN_STARTED_POLL
            );

            _systemAuditLogRepository.Add(systemAuditLog);

            await _unitOfWork.SaveChangesAsync();

            var response = poll.ToPollResponse();

            return Result<Poll_ResponseDTO>.Success(response);  
        }

        public async Task<Result<Election_ResponseDTO>> CompleteElectionAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                return Result<Election_ResponseDTO>.Failure(Error.NotFound(
                    nameof(ProblemDetails404ErrorTypes.Election_NotFound),
                    "Election not found."
                ));
            }

            if (election.Status == StatusEnum.Upcoming)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Complete_UpcomingElection),
                    "Election is still in the upcoming phase."
                ));
            }
            if (election.Status == StatusEnum.Completed)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Complete_CompletedElection),
                    "Election is already completed."
                ));
            }
            if (election.Status != StatusEnum.Voting)
            {
                return Result<Election_ResponseDTO>.Failure(Error.Failure(
                    nameof(ProblemDetails500ErrorTypes.Server_Error),
                    "Election is in an unknown phase."
                ));
            }

            election.EndVotingPhase();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string adminUsername = _currentUserServices.UserName!;

            SystemAuditLog systemAuditLog = SystemAuditLog.Create(
                adminId,
                adminUsername,
                SystemActionTypesEnum.ADMIN_ENDED_ELECTION_VOTING_PHASE
            );

            _systemAuditLogRepository.Add(systemAuditLog);

            // Calculate and store the election winners
            // Calculate and store the statistics for the completed election



            await _unitOfWork.SaveChangesAsync();

            var response = election.ToElectionResponse();

            return Result<Election_ResponseDTO>.Success(response);
        }

        public async Task<Result<Poll_ResponseDTO>> CompletePollAsync(int pollId)
        {
            var poll = await _pollRepository.GetByIdAsync(pollId);
            if (poll == null)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.NotFound(
                    nameof(ProblemDetails404ErrorTypes.Poll_NotFound),
                    "Poll not found."
                ));
            }
            if(poll.Status == StatusEnum.Upcoming)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Complete_UpcomingPoll),
                    "Poll is still in the upcoming phase."
                ));
            }
            if (poll.Status == StatusEnum.Completed)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Validation(
                    nameof(ProblemDetails400ErrorTypes.Cannot_Complete_CompletedPoll),
                    "Poll is already completed."
                ));
            }
            if (poll.Status != StatusEnum.Voting)
            {
                return Result<Poll_ResponseDTO>.Failure(Error.Failure(
                    nameof(ProblemDetails500ErrorTypes.Server_Error),
                    "Poll is in an unknown phase."
                ));
            }

            poll.EndPoll();

            int adminId = (int)_currentUserServices.ApplicationUserId!;
            string adminUsername = _currentUserServices.UserName!;

            SystemAuditLog systemAuditLog = SystemAuditLog.Create(
                adminId,
                adminUsername,
                SystemActionTypesEnum.ADMIN_ENDED_POLL
            );

            _systemAuditLogRepository.Add(systemAuditLog);


            // Calculate and store the poll won answer
            // Calculate and store the statistics for the completed poll



            await _unitOfWork.SaveChangesAsync();

            var response = poll.ToPollResponse();

            return Result<Poll_ResponseDTO>.Success(response);
        }

        public Task<Result<UserOperations_ResponseDTO>> GetUserByHashedDataAsync(UserCheckOrBanAccountByHashedData_RequestDTO requestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserOperations_ResponseDTO>> GetUserByUsernameAsync(UserCheckOrBanAccountByUserName_RequestDTO requestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserOperations_ResponseDTO>> ResetUserPasswordByUsernameAsync(UserResetPasswordByUserName_RequestDTO requestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserOperations_ResponseDTO>> BanUserByHashedDataAsync(UserCheckOrBanAccountByHashedData_RequestDTO requestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserOperations_ResponseDTO>> BanUserByUsernameAsync(UserCheckOrBanAccountByUserName_RequestDTO requestDTO)
        {
            throw new NotImplementedException();
        }

        

       
        public Task<PagedResult<SystemAuditLog_ResponseDTO>> GetPagedSystemAuditLogsAsync(SystemActionTypesEnum? actionTypesEnum, int? adminId, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        

        
    }

}