using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Vote vote)
        {
            _context.Votes.Add(vote);
        }

        public async Task<bool> IsVoteChoicesForVoteEqualFive(Vote vote)
        {
            // Relies on VoteChoices being loaded or checking against the DB
            return await _context.VoteChoices.CountAsync(vc => vc.VoteId == vote.Id) == 5;
        }

        public async Task<int> GetCountOfTotalVotesByElectionIdAsync(int electionId)
        {
            return await _context.Votes.CountAsync(v => v.ElectionId == electionId);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndGenderAsync(int electionId, char gender)
        {
            return await _context.Votes.CountAsync(v => v.ElectionId == electionId && v.VoterGender == gender);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndAgeRangeAsync(int electionId, int minAge, int maxAge)
        {
            return await _context.Votes
                .CountAsync(v => v.ElectionId == electionId && v.VoterAge >= minAge && v.VoterAge <= maxAge);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.Votes
                .CountAsync(v => v.ElectionId == electionId && v.Governorate == governorate);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndGenderAndGovernorateAsync(int electionId, char gender, GovernorateIdEnum governorate, CancellationToken cancellationToken)
        {
            return await _context.Votes
                .CountAsync(v => v.ElectionId == electionId && 
                                 v.VoterGender == gender && 
                                 v.Governorate == governorate, cancellationToken);
        }

        public async Task<int> GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(int electionId, int minAge, int maxAge, int governorate)
        {
            // FLAG: Interface expects `int governorate` but Entity uses `GovernorateIdEnum`. 
            // We cast the int back to the enum.
            var enumGovernorate = (GovernorateIdEnum)governorate;

            return await _context.Votes
                .CountAsync(v => v.ElectionId == electionId && 
                                 v.VoterAge >= minAge && 
                                 v.VoterAge <= maxAge && 
                                 v.Governorate == enumGovernorate);
        }
    }
}