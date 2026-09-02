using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ResponseDTOs
{
    public static class ResponseMappingExtensions
    {
        public static Election_ResponseDTO ToElectionResponse(this Election election)
        {
            return new Election_ResponseDTO
            {
                Id = election.Id,
                Name = election.Name,
                NominationStartDate = election.NominationStartDate,
                NominationEndDate = election.NominationEndDate,
                VotingStartDate = election.VotingStartDate,
                VotingEndDate = election.VotingEndDate,
                Status = election.Status
            };
        }

        public static Poll_ResponseDTO ToPollResponse(this Poll poll)
        {
            return new Poll_ResponseDTO
            {
                Id = poll.Id,
                Name = poll.Name,
                Question = poll.Question,
                StartDate = poll.StartDate,
                EndDate = poll.EndDate,
                Status = poll.Status,
                Answers = poll.PollAnswers.Select(a => new PollAnswerListDTO
                {
                    Id = a.Id,
                    Answer = a.Answer
                }).ToList()
            };
        }
    }
}
