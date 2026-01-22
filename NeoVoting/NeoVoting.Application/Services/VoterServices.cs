using Microsoft.AspNetCore.Http.HttpResults;
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
    public class VoterServices : IVoterServices
    {   
        private readonly IElectionRepository _electionRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IGovernmentSystemGateway _governmentSystemGateway;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoteRepository _voteRepository;
        private readonly IPublicVoteLogRepository _publicVoteLogRepository;
        private readonly IVoteChoiceRepository _voteChoice;

        public VoterServices(
            IElectionRepository electionRepository,
            ICandidateProfileRepository candidateProfileRepository,
            ICurrentUserServices currentUserServices,
            IGovernmentSystemGateway governmentSystemGateway,
            IUnitOfWork unitOfWork,
            IVoteRepository voteRepository,
            IPublicVoteLogRepository publicVoteLogRepository,
            IVoteChoiceRepository voteChoice
            )
        {
            _electionRepository = electionRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _currentUserServices = currentUserServices;
            _governmentSystemGateway = governmentSystemGateway;
            _unitOfWork = unitOfWork;
            _voteRepository = voteRepository;
            _publicVoteLogRepository = publicVoteLogRepository;
            _voteChoice = voteChoice;
        }

        public Task<Result<IReadOnlyList<CandidateProfile_ResponseDTO>>> GetPagedCandidatesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public  Task<Result<IReadOnlyList<CandidateProfile_ResponseDTO>>> GetPagedCandidatesByElectionIdAsync(Guid electionId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<PublicVoteLog_ResponseDTO>>> GetPagedPublicVoteLogsByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<PublicVoteLog_ResponseDTO>>> GetPagedPublicVoteLogsByElectionIdAsync(Guid electionId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<PublicVoteLog_ResponseDTO>> GetPublicVoteLogByVoteIdAsync(Guid electionId, Guid voteId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<VoterCastVote_ResponseDTO>> VoterCastVoteAsync(Guid electionId, VoterCastVote_RequestDTO request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
