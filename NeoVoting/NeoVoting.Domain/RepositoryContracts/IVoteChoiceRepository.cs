using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteChoiceRepository
    {
        Task<VoteChoice> AddVoteChoiceAsync(VoteChoice voteChoice, CancellationToken cancellationToken);

        Task<bool> IsVoteChoiceExistsByVoteIdAndCandidateProfileIdAsync(Guid voteId,Guid candidateProfileId , CancellationToken cancellationToken);

        void Delete(VoteChoice voteChoice);//in case Voted flag in Government System was not updated correctly to true we will soft delete the VoteChoice


        //vote count by candidate profile id(which includes election id and governorate id info)
        Task<int> GetTotalVoteChoicesCountByCandidateProfileIdAsync(Guid candidateProfileId, CancellationToken cancellationToken);

        // winners per governorate id per election id
        Task<IReadOnlyList<Guid>> GetTop5CandidatesIdsPerGovernorate(Guid electionId,int governorateId,CancellationToken cancellationToken);


    }
}