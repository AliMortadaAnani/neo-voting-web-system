using NeoVoting.Domain.EF_DTOs;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPollVoteRepository
    {
        void Add(PollVote vote);

        Task<int> CountByPollIdAsync(int pollId);

        Task<List<PollVote>> GetPagedByPollIdAsync(int pollId, int pageNumber, int pageSize);

        Task<PollVote?> GetByIdAsync(Guid id);

        Task<bool> IsPollAnswerExistByIdInPoll(int pollAnswerId, int pollId);
        Task<List<PollAnswerWithVotesDto>> GetResultsAsyncByPollId(int pollId);
    }
}