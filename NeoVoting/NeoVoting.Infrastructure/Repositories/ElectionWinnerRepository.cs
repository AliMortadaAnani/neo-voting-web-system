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

        public async Task<List<CandidateResultResponseDTO>> GetAllWinnersByElectionIdAsync(int electionId)
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
                .OrderByDescending(ew => ew.VoteCount ?? ew.CandidateProfile.VoteChoices.Count) // Order by VoteCount, using the count of VoteChoices as a fallback
                .Select(ew => new CandidateResultResponseDTO
                {
                    CandidateProfileId = ew.CandidateProfileId,
                    FirstName = ew.CandidateProfile.Candidate.FirstName,
                    LastName = ew.CandidateProfile.Candidate.LastName,
                    ProfilePhotoFilename = ew.CandidateProfile.ProfilePhotoFilename ?? string.Empty,
                    Governorate = ew.CandidateProfile.Candidate.Governorate,
                    VoteCount = ew.VoteCount ?? ew.CandidateProfile.VoteChoices.Count // Evaluated efficiently by EF Core as a SQL COUNT aggregate
                })
                .ToListAsync();
        }

        public async Task<List<CandidateResultResponseDTO>> GetAllWinnersByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate)
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
                               .OrderByDescending(ew => ew.VoteCount ?? ew.CandidateProfile.VoteChoices.Count) // Order by VoteCount, using the count of VoteChoices as a fallback
                .Select(ew => new CandidateResultResponseDTO
                {
                    CandidateProfileId = ew.CandidateProfileId,
                    FirstName = ew.CandidateProfile.Candidate.FirstName,
                    LastName = ew.CandidateProfile.Candidate.LastName,
                    ProfilePhotoFilename = ew.CandidateProfile.ProfilePhotoFilename ?? string.Empty,
                    Governorate = ew.CandidateProfile.Candidate.Governorate,
                    VoteCount = ew.VoteCount ?? ew.CandidateProfile.VoteChoices.Count // Evaluated efficiently by EF Core as a SQL COUNT aggregate
                })
                .ToListAsync();
        }

        public Task<bool> IsCandidateProfileWinnerExistByElectionIdAsync(int electionId, int candidateProfileId)
        {
            return _context.ElectionWinners
                .AnyAsync(ew => ew.CandidateProfileId == candidateProfileId && ew.CandidateProfile.ElectionId == electionId);
        }
    }
}