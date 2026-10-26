using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    // Interface is named IEventParticipation, so implementing exactly that
    public class EventParticipationRepository : IEventParticipationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventParticipationRepository> _logger;

        public EventParticipationRepository(ApplicationDbContext context, ILogger<EventParticipationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(EventParticipation participation)
        {
            _logger.LogInformation("Recording event participation for voter {VoterId}, election {ElectionId}, poll {PollId}.", participation.VoterId, participation.ElectionId, participation.PollId);
            _context.EventParticipations.Add(participation);
        }

        public async Task<bool> HasVoterVotedByVoterIdAndElectionIdAsync(int voterId, int electionId)
        {
            _logger.LogInformation("Checking whether voter {VoterId} has voted in election {ElectionId}.", voterId, electionId);
            return await _context.EventParticipations
                .AnyAsync(ep => ep.VoterId == voterId && ep.ElectionId == electionId);
        }

        public async Task<bool> HasVoterVotedByVoterIdAndPollIdAsync(int voterId, int pollId)
        {
            _logger.LogInformation("Checking whether voter {VoterId} has voted in poll {PollId}.", voterId, pollId);
            return await _context.EventParticipations
                .AnyAsync(ep => ep.VoterId == voterId && ep.PollId == pollId);
        }
    }
}