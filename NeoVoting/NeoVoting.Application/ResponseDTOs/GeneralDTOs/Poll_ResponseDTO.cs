using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.GeneralDTOs
{
    public class Poll_ResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Question { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public StatusEnum Status { get; set; }

        public List<PollAnswerDTO>? Answers { get; set; } = new List<PollAnswerDTO>();
    }

    public class PollAnswerDTO
    {
        public int? Id { get; set; }
        public string? Answer { get; set; }
        public int? VotesCount { get; set; }
    }
}