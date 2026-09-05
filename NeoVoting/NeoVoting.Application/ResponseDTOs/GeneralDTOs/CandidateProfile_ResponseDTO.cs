namespace NeoVoting.Application.ResponseDTOs.GeneralDTOs
{
    public class CandidateProfile_ResponseDTO
    {
        public int? CandidateProfileId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public char? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? NominationReasons { get; set; }
        public int? VotesCount { get; set; }
    }
}