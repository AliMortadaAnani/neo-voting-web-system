namespace NeoVoting.Domain.Entities
{
    public class PollStatistics
    {
        public int Id { get; set; } //ep
        public int PollId { get; set; } //p

        public int? RegisteredVotersCount { get; set; } //ep // calculated at the end of the election/poll
        public int? ActualVotersCount { get; set; } //ep // from Vote tables
        public double? ParticipationPercentage { get; set; } //ep
        public Poll Poll { get; set; }

        public PollStatistics()
        {
            Poll = null!;
        }
    }
}