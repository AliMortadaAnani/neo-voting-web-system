namespace NeoVoting.Application.ResponseDTOs.GeneralDTOs
{
    public class CompletedPollStatistics_ResponseDTO
    {
        public int PollId { get; set; }
        public string? PollName { get; set; }
        public string? Question { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<CompletedPollAnswerListDTO>? Answers { get; set; } = new List<CompletedPollAnswerListDTO>();

        public int? RegisteredVotersCount { get; set; }
        public int? ActualVotersCount { get; set; }
        public double? ParticipationPercentage { get; set; }

        public string? WinnerAnswer { get; set; }

        public int? WinnerVotesCount { get; set; }

        public double? WinnerVotesPercentage { get; set; }
    }

    public class CompletedPollAnswerListDTO
    {
        public int? Id { get; set; }
        public string? Answer { get; set; }

        public int? VotesCount { get; set; }

        public double? VotesPercentage { get; set; }
    }
}