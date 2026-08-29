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

        ///
        ///

        public async Task<int> GetCountOfTotalVotesByElectionIdAsync(int electionId)
        {
            return await _context.EventParticipations.CountAsync(ep => ep.ElectionId == electionId);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndGenderAsync(int electionId, char gender)
        {
            return await _context.EventParticipations.CountAsync(ep => ep.ElectionId == electionId && ep.Voter.Gender == gender);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndAgeRangeAsync(int electionId, int minAge, int maxAge)
        {
            // Use DateTime.Today to align with local calendar dates rather than UTC shifts
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxDate = today.AddYears(-minAge);
            var minDate = today.AddYears(-(maxAge + 1)).AddDays(1);

            return await _context.EventParticipations
                .CountAsync(ep => ep.ElectionId == electionId && ep.Voter.DateOfBirth >= minDate && ep.Voter.DateOfBirth <= maxDate);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.EventParticipations
                .CountAsync(v => v.ElectionId == electionId && v.Voter.Governorate == governorate);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndGenderAndGovernorateAsync(int electionId, char gender, GovernorateIdEnum governorate)
        {
            return await _context.EventParticipations
                .CountAsync(v => v.ElectionId == electionId &&
                                 v.Voter.Gender == gender &&
                                 v.Voter.Governorate == governorate);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(int electionId, int minAge, int maxAge, GovernorateIdEnum governorate)
        {
            // Use DateTime.Today to align with local calendar dates rather than UTC shifts
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxDate = today.AddYears(-minAge);
            var minDate = today.AddYears(-(maxAge + 1)).AddDays(1);

            return await _context.EventParticipations
                .CountAsync(v => v.ElectionId == electionId &&
                                 v.Voter.DateOfBirth >= minDate &&
                                 v.Voter.DateOfBirth <= maxDate &&
                                 v.Voter.Governorate == governorate);
        }
    }
}