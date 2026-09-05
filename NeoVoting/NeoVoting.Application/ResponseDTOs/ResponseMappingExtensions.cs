using NeoVoting.Application.ResponseDTOs.AdminDTOs;
using NeoVoting.Application.ResponseDTOs.GeneralDTOs;
using NeoVoting.Domain.EF_DTOs;
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

        public static SystemAuditLog_ResponseDTO ToSystemAuditLogResponse(this SystemAuditLog log)
        {
            return new SystemAuditLog_ResponseDTO
            {
                Id = log.Id,
                ActionType = log.ActionType,
                Details = log.Details,
                AdminId = log.AdminId,
                Username = log.Username,
                TimestampUTC = log.TimestampUTC
            };
        }

        public static CandidateProfile_ResponseDTO ToCandidateProfileResponse(
                                        this CandidateProfile profile,
                                        int? VotesCount)
        {
            var dto = new CandidateProfile_ResponseDTO
            {
                CandidateProfileId = profile.Id,
                FirstName = profile.Candidate.FirstName,
                LastName = profile.Candidate.LastName,
                Gender = profile.Candidate.Gender,
                DateOfBirth = profile.Candidate.DateOfBirth,
                NominationReasons = profile.NominationReasons
            };

            // Only add VotesCount if it has a value
            if (VotesCount.HasValue)
            {
                dto.VotesCount = VotesCount.Value;
            }

            return dto;
        }

        public static CandidateProfile_ResponseDTO ToCandidateProfileResultsResponse(
                                        this CandidateProfileWithVotesDto profileResults)
        {
            var dto = new CandidateProfile_ResponseDTO
            {
                CandidateProfileId = profileResults.CandidateProfile.Id,
                FirstName = profileResults.CandidateProfile.Candidate.FirstName,
                LastName = profileResults.CandidateProfile.Candidate.LastName,
                Gender = profileResults.CandidateProfile.Candidate.Gender,
                DateOfBirth = profileResults.CandidateProfile.Candidate.DateOfBirth,
                NominationReasons = profileResults.CandidateProfile.NominationReasons,
                VotesCount = profileResults.TotalVotes
            };

            
            return dto;
        }

        // Option A: If you have a list or query result containing the answer and its specific vote count
        public static Poll_ResponseDTO ToPollResponse(
            this Poll poll,
            List<PollAnswerWithVotesDto> answersWithVotes)
        {
            return new Poll_ResponseDTO
            {
                Id = poll.Id,
                Name = poll.Name,
                Question = poll.Question,
                StartDate = poll.StartDate,
                EndDate = poll.EndDate,
                Status = poll.Status,
                Answers = answersWithVotes.Select(av => new PollAnswerDTO
                {
                    Id = av.pollAnswer.Id,
                    Answer = av.pollAnswer.Answer,
                    VotesCount = av.TotalVotes
                }).ToList()
            };
        }
    }
}