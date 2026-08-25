using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionWinnerRepository : IElectionWinnerRepository
    {
        private readonly ApplicationDbContext _context;

        public ElectionWinnerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(ElectionWinner winner)
        {
            _context.ElectionWinners.Add(winner);
        }

        public async Task<List<ElectionWinner>> GetAllWinnersByElectionIdAsync(int electionId)
        {
            return await _context.ElectionWinners
                .Include(ew => ew.CandidateProfile)
                .ThenInclude(cp => cp.Candidate)
                .Where(ew => ew.CandidateProfile.ElectionId == electionId)
                .ToListAsync();
        }

        public async Task<List<ElectionWinner>> GetAllWinnersByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.ElectionWinners
                .Include(ew => ew.CandidateProfile)
                .ThenInclude(cp => cp.Candidate)
                .Where(ew => ew.CandidateProfile.ElectionId == electionId && 
                            ew.CandidateProfile.Candidate.Governorate == governorate)
                .ToListAsync();
        }
    }
}