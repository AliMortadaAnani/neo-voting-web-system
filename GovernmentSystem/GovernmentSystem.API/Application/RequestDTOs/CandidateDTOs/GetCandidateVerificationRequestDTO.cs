namespace GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs
{
    public class GetCandidateVerificationRequestDTO
    {
        public string? NationalId { get; set; }

        public string? NominationToken { get; set; }
    }
}