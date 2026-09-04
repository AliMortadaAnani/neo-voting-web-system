using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.EF_DTOs;
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

        public async Task<List<CandidateResultResponseEF_DTO>> GetPagedWinnersByElectionIdAsync(
    int electionId,
    GovernorateIdEnum? governorate,
    int pageNumber,
    int pageSize)
        {
            var query = _context.ElectionWinners
                .AsNoTracking()
                .Where(ew => ew.CandidateProfile.ElectionId == electionId);

            // Apply governorate filter conditionally if provided
            if (governorate.HasValue)
            {
                query = query.Where(ew => ew.CandidateProfile.Candidate.Governorate == governorate.Value);
            }

            return await query
                .OrderByDescending(ew => ew.VoteCount ?? ew.CandidateProfile.VoteChoices.Count)
                .Select(ew => new CandidateResultResponseEF_DTO
                {
                    CandidateProfileId = ew.CandidateProfileId,
                    FirstName = ew.CandidateProfile.Candidate.FirstName,
                    LastName = ew.CandidateProfile.Candidate.LastName,
                    ProfilePhotoFilename = ew.CandidateProfile.ProfilePhotoFilename ?? string.Empty,
                    Governorate = ew.CandidateProfile.Candidate.Governorate,
                    VoteCount = ew.VoteCount ?? ew.CandidateProfile.VoteChoices.Count
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountWinnersByElectionIdAsync(
            int electionId,
            GovernorateIdEnum? governorate)
        {
            var query = _context.ElectionWinners
                .Where(ew => ew.CandidateProfile.ElectionId == electionId);

            if (governorate.HasValue)
            {
                query = query.Where(ew => ew.CandidateProfile.Candidate.Governorate == governorate.Value);
            }

            return await query.CountAsync();
        }

        public async Task<bool> IsCandidateProfileWinnerExistByElectionIdAsync(int electionId, int candidateProfileId)
        {
            return await _context.ElectionWinners
                .AnyAsync(ew => ew.CandidateProfileId == candidateProfileId && ew.CandidateProfile.ElectionId == electionId);
        }

        public async Task<List<ElectionWinner>> GetAllWinnersByElectionIdAsync(int electionId, GovernorateIdEnum? governorate = null)
        {
            var query = _context.ElectionWinners
                .Include(ew => ew.CandidateProfile)
                .ThenInclude(cp => cp.Candidate)
                .AsNoTracking()
                .Where(ew => ew.CandidateProfile.ElectionId == electionId);

            // Conditionally filter by governorate if provided and not null
            if (governorate.HasValue)
            {
                query = query.Where(ew => ew.CandidateProfile.Candidate.Governorate == governorate.Value);
            }

            return await query.ToListAsync();
        }
    }
}