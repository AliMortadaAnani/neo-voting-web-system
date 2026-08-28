namespace NeoVoting.Application.RequestDTOs
{
    public class CandidateProfileUpdate_RequestDTO
    {
        public string? Goals { get; set; }
        public string? NominationReasons { get; set; }
        public Guid? NationalId { get; set; }

        public Guid? NominationToken { get; set; }
    }
}