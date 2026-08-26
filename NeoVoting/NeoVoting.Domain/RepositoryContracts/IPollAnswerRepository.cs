using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPollAnswerRepository
    {
        void Add(PollAnswer answer);
        Task<bool> IsPollAnswerExistsAsync(int pollId, string answerText);
        Task<List<PollAnswer>> GetAllAnswersByPollIdAsync(int pollId); // Not paged since we expect only 5 answers per poll
    }
}
