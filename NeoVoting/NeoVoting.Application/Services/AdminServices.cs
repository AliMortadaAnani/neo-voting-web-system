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
        private readonly IVoteChoiceRepository _voteChoiceRepository;
        private readonly IVoterRepository _voterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventParticipationRepository _eventParticipationRepository;
        private readonly IPollVoteRepository _pollVoteRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository; 
        public AdminServices(IElectionRepository electionRepository, IPollRepository pollRepository, ISystemAuditLogRepository systemAuditLogRepository, IElectionWinnerRepository electionWinnerRepository, IPollAnswerRepository pollAnswerRepository, IElectionStatisticsRepository electionStatisticsRepository, IPollStatisticsRepository pollStatisticsRepository, IUnitOfWork unitOfWork , ICurrentUserServices currentUserServices , IVoteChoiceRepository voteChoiceRepository,
            IVoterRepository voterRepository,
            IEventParticipationRepository eventParticipationRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IPollVoteRepository pollVoteRepository
            )
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
            _voteChoiceRepository = voteChoiceRepository;
            _voterRepository = voterRepository;
            _eventParticipationRepository = eventParticipationRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _pollVoteRepository = pollVoteRepository;
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

            var beirutWinners = await _voteChoiceRepository.GetTop5CandidatesProfilesPerGovernorateAsync(electionId, GovernorateIdEnum.Beirut);

            var mountLebanonWinners = await _voteChoiceRepository.GetTop5CandidatesProfilesPerGovernorateAsync(electionId, GovernorateIdEnum.MountLebanon);

            var southWinners = await _voteChoiceRepository.GetTop5CandidatesProfilesPerGovernorateAsync(electionId, GovernorateIdEnum.South);

            var eastWinners = await _voteChoiceRepository.GetTop5CandidatesProfilesPerGovernorateAsync(electionId, GovernorateIdEnum.East);

            var northWinners = await _voteChoiceRepository.GetTop5CandidatesProfilesPerGovernorateAsync(electionId, GovernorateIdEnum.North);

            // 2. Combine all lists into a single collection (up to 25 items total)
            var allWinners = new List<CandidateResultResponseEF_DTO>();
            allWinners.AddRange(beirutWinners);
            allWinners.AddRange(mountLebanonWinners);
            allWinners.AddRange(southWinners);
            allWinners.AddRange(northWinners);
            allWinners.AddRange(eastWinners);

            // 3. Loop through each candidate and insert them if they don't already exist as a winner for this election
            int addedCount = 0;

            foreach (var winnerDto in allWinners)
            {
                // Check if this candidate profile is already marked as a winner for this election
                bool exists = await _electionWinnerRepository.IsCandidateProfileWinnerExistByElectionIdAsync(electionId, winnerDto.CandidateProfileId);

                if (!exists)
                {
                    // Create the ElectionWinner entity using its factory method or constructor
                    var electionWinner = ElectionWinner.Create(
                        winnerDto.CandidateProfileId,
                        winnerDto.VoteCount
                    );

                    // Add to context via repository
                    _electionWinnerRepository.Add(electionWinner);
                    addedCount++;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Calculate Stats

            // 1. Define standard parliament start/end dates (Today to 5 years later, based on your prompt)
            var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = startDate.AddYears(5);

            var listToSave = new List<ElectionStatistics>();

            // =========================================================================
            // PART A: BUILD GLOBAL STATISTICS (Governorate = null)
            // =========================================================================
            var globalStats = new ElectionStatistics(electionId, governorate: null, startDate, endDate);

            // 1. Populate Voters Data (Global)
            int totalRegistered = await _voterRepository.CountAsync(); // Total registered in system (or filter by election if applicable)
            int regMales = await _voterRepository.CountByGenderAsync('M'); // handle case-insensitivity if needed ('m'/'M')
            int regFemales = await _voterRepository.CountByGenderAsync('F');
            int reg18_29 = await _voterRepository.CountsByAgeRangeAsync(18, 29);
            int reg30_45 = await _voterRepository.CountsByAgeRangeAsync(30, 45);
            int reg46_64 = await _voterRepository.CountsByAgeRangeAsync(46, 64);
            int reg65Plus = await _voterRepository.CountsByAgeRangeAsync(65, 120);

            int totalActual = await _eventParticipationRepository.GetCountOfTotalVotesByElectionIdAsync(electionId);
            int actMales = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndGenderAsync(electionId, 'M');
            int actFemales = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndGenderAsync(electionId, 'F');
            int act18_29 = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgeRangeAsync(electionId, 18, 29);
            int act30_45 = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgeRangeAsync(electionId, 30, 45);
            int act46_64 = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgeRangeAsync(electionId, 46, 64);
            int act65Plus = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgeRangeAsync(electionId, 65, 120);

            globalStats.PopulateVoterData(
                totalRegistered, regMales, regFemales, reg18_29, reg30_45, reg46_64, reg65Plus,
                totalActual, actMales, actFemales, act18_29, act30_45, act46_64, act65Plus
            );

            // 2. Populate Candidates Data (Global)
            int totalNominated = await _candidateProfileRepository.CountByElectionIdAsync(electionId, governorate: null);
            int nomMales = await _candidateProfileRepository.CountByElectionIdAndGenderAsync(electionId, 'M');
            int nomFemales = await _candidateProfileRepository.CountByElectionIdAndGenderAsync(electionId, 'F');
            int nom18_29 = await _candidateProfileRepository.CountsByElectionIdAndAgeRangeAsync(electionId, 18, 29);
            int nom30_45 = await _candidateProfileRepository.CountsByElectionIdAndAgeRangeAsync(electionId, 30, 45);
            int nom46_64 = await _candidateProfileRepository.CountsByElectionIdAndAgeRangeAsync(electionId, 46, 64);
            int nom65Plus = await _candidateProfileRepository.CountsByElectionIdAndAgeRangeAsync(electionId, 65, 120);

            globalStats.PopulateCandidatesData(totalNominated, nomMales, nomFemales, nom18_29, nom30_45, nom46_64, nom65Plus);

            // 3. Populate Winners Data (Global)
            var allElectionWinners = await _electionWinnerRepository.GetAllWinnersByElectionIdAsync(electionId);

            globalStats.PopulateWinnersData(allElectionWinners);
            listToSave.Add(globalStats);


            // =========================================================================
            // PART B: BUILD PER-GOVERNORATE STATISTICS (5 Governorates)
            // =========================================================================
            var governorates = new[]
            {
        GovernorateIdEnum.Beirut,
        GovernorateIdEnum.MountLebanon,
        GovernorateIdEnum.South,
        GovernorateIdEnum.North,
        GovernorateIdEnum.East
    };

            foreach (var gov in governorates)
            {
                var govStats = new ElectionStatistics(electionId, governorate: gov, startDate, endDate);

                // Voters (Gov)
                int govRegTotal = await _voterRepository.CountByGovernorateAsync(gov);
                int govRegMales = await _voterRepository.CountByGovernorateAndGenderAsync(gov, 'M');
                int govRegFemales = await _voterRepository.CountByGovernorateAndGenderAsync(gov, 'F');
                int govReg18_29 = await _voterRepository.CountByGovernorateAndAgeRangeAsync(gov, 18, 29);
                int govReg30_45 = await _voterRepository.CountByGovernorateAndAgeRangeAsync(gov, 30, 45);
                int govReg46_64 = await _voterRepository.CountByGovernorateAndAgeRangeAsync(gov, 46, 64);
                int govReg65Plus = await _voterRepository.CountByGovernorateAndAgeRangeAsync(gov, 65, 120);

                int govActTotal = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndGovernorateAsync(electionId, gov);
                int govActMales = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndGenderAndGovernorateAsync(electionId, 'M', gov);
                int govActFemales = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndGenderAndGovernorateAsync(electionId, 'F', gov);
                int govAct18_29 = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(electionId, 18, 29, gov);
                int govAct30_45 = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(electionId, 30, 45, gov);
                int govAct46_64 = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(electionId, 46, 64, gov);
                int govAct65Plus = await _eventParticipationRepository.GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(electionId, 65, 120, gov);

                govStats.PopulateVoterData(
                    govRegTotal, govRegMales, govRegFemales, govReg18_29, govReg30_45, govReg46_64, govReg65Plus,
                    govActTotal, govActMales, govActFemales, govAct18_29, govAct30_45, govAct46_64, govAct65Plus
                );

                // Candidates (Gov)
                int govNomTotal = await _candidateProfileRepository.CountByElectionIdAsync(electionId, gov);
                int govNomMales = await _candidateProfileRepository.CountByElectionIdAndGovernorateAndGenderAsync(electionId, gov, 'M');
                int govNomFemales = await _candidateProfileRepository.CountByElectionIdAndGovernorateAndGenderAsync(electionId, gov, 'F');
                int govNom18_29 = await _candidateProfileRepository.CountByElectionIdAndGovernorateAndAgeRangeAsync(electionId, gov, 18, 29);
                int govNom30_45 = await _candidateProfileRepository.CountByElectionIdAndGovernorateAndAgeRangeAsync(electionId, gov, 30, 45);
                int govNom46_64 = await _candidateProfileRepository.CountByElectionIdAndGovernorateAndAgeRangeAsync(electionId, gov, 46, 64);
                int govNom65Plus = await _candidateProfileRepository.CountByElectionIdAndGovernorateAndAgeRangeAsync(electionId, gov, 65, 120);

                govStats.PopulateCandidatesData(govNomTotal, govNomMales, govNomFemales, govNom18_29, govNom30_45, govNom46_64, govNom65Plus);

                // Winners (Gov)
                var govWinners = allElectionWinners
                    .Where(w => w.CandidateProfile.Candidate.Governorate == gov)
                    .ToList();

                govStats.PopulateWinnersData(govWinners);
                listToSave.Add(govStats);
            }

            // =========================================================================
            // PART C: ADD TO CONTEXT AND SAVE
            // =========================================================================
            // Assuming you have an ElectionStatistics repository or direct context usage:
            foreach (var stats in listToSave)
            {
                _electionStatisticsRepository.Add(stats);
            }

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


            await _unitOfWork.SaveChangesAsync();

            // Calculate and store the statistics for the completed poll
            // minimalistic statistics for the poll, since it's not as complex as elections and we can get stats on the fly

            var pollStats = new PollStatistics();

            pollStats.PollId = pollId;
            pollStats.RegisteredVotersCount = await _voterRepository.CountAsync();
            pollStats.ActualVotersCount = await _pollVoteRepository.CountByPollIdAsync(pollId);
            pollStats.ParticipationPercentage = pollStats.RegisteredVotersCount > 0 ? (double)pollStats.ActualVotersCount / pollStats.RegisteredVotersCount * 100 : 0;

            _pollStatisticsRepository.Add(pollStats);

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