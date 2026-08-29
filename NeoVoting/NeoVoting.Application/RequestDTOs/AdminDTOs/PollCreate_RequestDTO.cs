namespace NeoVoting.Application.RequestDTOs.AdminDTOs
{
    public class PollCreate_RequestDTO
    {
        public string? Name { get; set; }
        public string? Question { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<string>? Answers { get; set; } = new List<string>();
    }
}