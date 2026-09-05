using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Domain.Entities;

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
                Answers = poll.PollAnswers.Select(a => new PollAnswerDTO
                {
                    Id = a.Id,
                    Answer = a.Answer
                }).ToList()
            };
        }
    }
}