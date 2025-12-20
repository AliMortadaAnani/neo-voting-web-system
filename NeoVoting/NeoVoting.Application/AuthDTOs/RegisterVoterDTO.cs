namespace NeoVoting.Application.AuthDTOs
{
    public class RegisterVoterDTO
    {
        public string? UserName { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }

        public Guid? NationalId { get; set; }

        public Guid? VotingToken { get; set; }
    }
}