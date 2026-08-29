namespace NeoVoting.Application.RequestDTOs.AuthDTOs
{
    public class RegisterVoterOrCandidate_RequestDTO
    {
        public string? Username { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }

        public string? NationalId { get; set; }

        public string? VotingOrNominationToken { get; set; }
    }
}