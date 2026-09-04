using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    // Interface is named IEventParticipation, so implementing exactly that
    public class EventParticipationRepository : IEventParticipationRepository
    {
        private readonly ApplicationDbContext _context;

        public EventParticipationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(EventParticipation participation)
        {
            _context.EventParticipations.Add(participation);
        }

        public async Task<bool> HasVoterVotedByVoterIdAndElectionIdAsync(int voterId, int electionId)
        {
            return await _context.EventParticipations
                .AnyAsync(ep => ep.VoterId == voterId && ep.ElectionId == electionId);
        }

        public async Task<bool> HasVoterVotedByVoterIdAndPollIdAsync(int voterId, int pollId)
        {
            return await _context.EventParticipations
                .AnyAsync(ep => ep.VoterId == voterId && ep.PollId == pollId);
        }

        
    }
}