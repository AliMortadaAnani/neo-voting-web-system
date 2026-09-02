using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.ResponseDTOs.AdminDTOs
{
    public class Poll_ResponseDTO
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Question { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public StatusEnum? Status { get; set; }
        public List<PollAnswerListDTO>? Answers { get; set; } = new List<PollAnswerListDTO>();
    }

    public class PollAnswerListDTO
    {
        public int? Id { get; set; }
        public string? Answer { get; set; }
    }
}