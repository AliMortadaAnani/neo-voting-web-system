using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteChoiceRepository
    {
        void Add(VoteChoice voteChoice);

        Task<bool> IsVoteChoiceExistsByVoteIdAndCandidateProfileIdAsync(int voteId,int candidateProfileId);

      

        //vote count by candidate profile id(which includes election id and governorate id info)
        Task<int> GetCountOfTotalVoteChoicesByCandidateProfileIdAsync(int candidateProfileId);

        // winners per governorate id per election id
        Task<List<CandidateProfile>> GetTop5CandidatesProfilesPerGovernorate(int electionId,GovernorateIdEnum governorate);


    }
}