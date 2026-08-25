using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPollVoteRepository
    {
        void Add(PollVote vote);

        Task<int> CountByPollIdAsync(int pollId);
        Task<PollVote?> GetByPollIdAndPollVoteIdAsync(int pollId, int pollVoteId);

        Task<List<PollVote>> GetPagedByPollIdAsync(int pollId, int pageNumber, int pageSize);
        
        Task<List<PollAnswer>> GetResultsAsyncByPollId(int pollId);

        Task<PollAnswer?> GetWinnerAnswerByPollIdAsync(int pollId);
    }
}
