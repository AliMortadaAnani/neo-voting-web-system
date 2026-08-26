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
                .AsNoTracking()
                // 1. Grab CandidateProfile, then its Candidate, then its User
                .Include(ew => ew.CandidateProfile)
                    .ThenInclude(profile => profile.Candidate)
                        .ThenInclude(candidate => candidate.User)
                // 2. Grab CandidateProfile again to get the Election
                .Include(ew => ew.CandidateProfile)
                    .ThenInclude(profile => profile.Election)
                .Where(ew => ew.CandidateProfile.ElectionId == electionId)
                .ToListAsync();
        }

        public async Task<List<ElectionWinner>> GetAllWinnersByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate)
        {
            return await _context.ElectionWinners
                .AsNoTracking()
                // 1. Grab CandidateProfile, then its Candidate, then its User
                .Include(ew => ew.CandidateProfile)
                    .ThenInclude(profile => profile.Candidate)
                        .ThenInclude(candidate => candidate.User)
                // 2. Grab CandidateProfile again to get the Election
                .Include(ew => ew.CandidateProfile)
                    .ThenInclude(profile => profile.Election)
                .Where(ew => ew.CandidateProfile.ElectionId == electionId &&
                            ew.CandidateProfile.Candidate.Governorate == governorate)
                .ToListAsync();
        }
    }
}